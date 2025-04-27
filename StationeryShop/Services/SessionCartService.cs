using System.Text.Json;
using StationeryShop.DTOs.Cart;
using StationeryShop.Models;
using StationeryShop.Data;
using Microsoft.EntityFrameworkCore;
using StationeryShop.Services;

namespace StationeryShop.Services
{
    public class SessionCartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StationeryDbContext _db;
        private const string CartSessionKey = "Cart";

        public SessionCartService(IHttpContextAccessor httpContextAccessor, StationeryDbContext db)
        {
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }

        private List<CartItemDto> GetCartItemsFromSession()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = session.GetString(CartSessionKey);
            return cartJson == null
                ? new List<CartItemDto>()
                : JsonSerializer.Deserialize<List<CartItemDto>>(cartJson);
        }

        private void SaveCartItemsToSession(List<CartItemDto> items)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString(CartSessionKey, JsonSerializer.Serialize(items));
        }

        public async Task<CartResponseDto> GetCartAsync()
        {
            var sessionItems = GetCartItemsFromSession();

            if (!sessionItems.Any())
            {
                throw new InvalidOperationException("Корзина пуста");
            }

            var productIds = sessionItems.Select(i => i.ProductId).ToList();
            var products = await _db.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var items = sessionItems.Select(item =>
            {
                var product = products[item.ProductId];
                return new CartResponseDto.CartItemDetailDto
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price * item.Quantity,
                    Quantity = item.Quantity
                };
            }).ToList();

            return new CartResponseDto
            {
                Items = items,
                TotalPrice = items.Sum(i => i.Price * i.Quantity)
            };
        }

        public async Task AddToCartAsync(CartItemDto itemDto)
        {
            var product = await _db.Products.FindAsync(itemDto.ProductId);
            if (product == null)
            {
                throw new ArgumentException("Товар не найден");
            }

            var sessionItems = GetCartItemsFromSession();
            var existingItem = sessionItems.FirstOrDefault(i => i.ProductId == itemDto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += 1;
            }
            else
            {
                sessionItems.Add(new CartItemDto
                {
                    ProductId = itemDto.ProductId,
                    Quantity = 1
                });
            }

            SaveCartItemsToSession(sessionItems);
        }

        public void RemoveFromCart(int productId)
        {
            var sessionItems = GetCartItemsFromSession();
            var itemToRemove = sessionItems.FirstOrDefault(i => i.ProductId == productId)
                ?? throw new KeyNotFoundException("Товар не найден в корзине");

            sessionItems.Remove(itemToRemove);
            SaveCartItemsToSession(sessionItems);
        }

        public async Task MergeWithUserCartAsync(int clientId)
        {
            var sessionItems = GetCartItemsFromSession();

            if (!sessionItems.Any()) return;

            var cartService = new CartService(_db);
            var userCart = await _db.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.ClientId == clientId);

            if (userCart == null)
            {
                userCart = new Cart { ClientId = clientId };
                _db.Carts.Add(userCart);
                await _db.SaveChangesAsync();
            }

            foreach (var item in sessionItems)
            {
                var existingItem = userCart.CartItems.FirstOrDefault(ci => ci.ProductId == item.ProductId);

                if (existingItem != null)
                {
                    existingItem.Quantity += item.Quantity;
                }
                else
                {
                    userCart.CartItems.Add(new CartItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });
                }
            }

            await _db.SaveChangesAsync();

            //очищаем сессионную корзину после объединения
            _httpContextAccessor.HttpContext.Session.Remove(CartSessionKey);
        }

        public DecreaseQuantityResult DecreaseQuantity(int productId)
        {
            var sessionItems = GetCartItemsFromSession();
            var item = sessionItems.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
            {
                throw new KeyNotFoundException("Товар не найден в корзине");
            }

            var oldQuantity = item.Quantity;
            bool removed = false;

            if (item.Quantity > 1)
            {
                item.Quantity--;
            }
            else
            {
                sessionItems.Remove(item);
                removed = true;
            }

            SaveCartItemsToSession(sessionItems);

            return new DecreaseQuantityResult
            {
                Removed = removed,
                OldQuantity = oldQuantity,
                NewQuantity = removed ? 0 : item.Quantity
            };
        }
    }
}