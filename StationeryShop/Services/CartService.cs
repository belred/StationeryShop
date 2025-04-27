using StationeryShop.Models;
using StationeryShop.Data;
using StationeryShop.DTOs.Cart;
using Microsoft.EntityFrameworkCore;
using static StationeryShop.DTOs.Cart.CartResponseDto;

namespace StationeryShop.Services
{
    public class CartService
    {
        private readonly StationeryDbContext _db;

        public CartService(StationeryDbContext db)
        {
            _db = db;
        }

        public async Task<CartResponseDto> GetCartAsync(int clientId)
        {
            var cart = await _db.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.ClientId == clientId);

            if (cart == null)
            {
                throw new KeyNotFoundException("Корзина не найдена");
            }

            if (!cart.CartItems.Any())
            {
                throw new InvalidOperationException("Корзина пуста");
            }

            return new CartResponseDto
            {
                Items = cart.CartItems.Select(ci => new CartItemDetailDto
                {
                    ProductId = ci.ProductId,
                    Name = ci.Product.Name,
                    Price = ci.Product.Price * ci.Quantity,
                    Quantity = ci.Quantity
                }).ToList(),
                TotalPrice = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity)
            };
        }

        public async Task AddToCartAsync(int clientId, CartItemDto itemDto)
        {
            var cart = await _db.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.ClientId == clientId)
                ?? throw new KeyNotFoundException("Корзина не найдена");

            var product = await _db.Products.FindAsync(itemDto.ProductId);
            if (product == null)
            {
                throw new ArgumentException("Товар не найден");
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == itemDto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += 1;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = 1
                });
            }

            await _db.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int clientId, int productId)
        {
            var cart = await _db.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.ClientId == clientId)
                ?? throw new KeyNotFoundException("Корзина не найдена");

            var itemToRemove = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId)
                ?? throw new KeyNotFoundException("Товар не найден в корзине");

            cart.CartItems.Remove(itemToRemove);
            await _db.SaveChangesAsync();
        }

        public async Task<DecreaseQuantityResult> DecreaseQuantityAsync(int clientId, int productId)
        {
            var cart = await _db.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.ClientId == clientId)
            ?? throw new KeyNotFoundException("Корзина не найдена");

            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId)
                ?? throw new KeyNotFoundException("Товар не найден в корзине");

            var oldQuantity = item.Quantity;
            bool removed = false;

            if (item.Quantity > 1)
            {
                item.Quantity--;
            }
            else
            {
                cart.CartItems.Remove(item);
                removed = true;
            }

            await _db.SaveChangesAsync();
            return new DecreaseQuantityResult
            {
                Removed = removed,
                OldQuantity = oldQuantity,
                NewQuantity = removed ? 0 : item.Quantity
            };
        }
    }

    public class DecreaseQuantityResult
    {
        public bool Removed { get; set; }
        public int OldQuantity { get; set; }
        public int NewQuantity { get; set; }
    }
}
