using StationeryShop.Models;
using StationeryShop.DTOs.Order;
using Microsoft.EntityFrameworkCore;
using static StationeryShop.DTOs.Order.OrderResponseDto;

namespace StationeryShop.Services
{
    public class OrderService
    {
        private readonly StationeryDbContext _db;
        private readonly CartService _cartService;

        public OrderService(StationeryDbContext db, CartService cartService)
        {
            _db = db;
            _cartService = cartService;
        }

        public async Task<int> CreateOrderAsync(int clientId, OrderCreateDto dto)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrWhiteSpace(dto.Address))
                    throw new ArgumentException("Адрес доставки обязателен");

                var cart = await _db.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.ClientId == clientId);

                if (cart == null || !cart.CartItems.Any())
                    throw new InvalidOperationException("Невозможно создать заказ: корзина пуста");

                var order = new Order
                {
                    ClientId = clientId,
                    Address = dto.Address,
                    Status = "Создан",
                    OrderDate = DateTime.UtcNow,
                    OrderItems = cart.CartItems.Select(ci => new OrderItem
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        Price = ci.Product.Price
                    }).ToList()
                };

                order.TotalAmount = order.OrderItems.Sum(oi => oi.Quantity * oi.Price);

                await _db.Orders.AddAsync(order);
                cart.CartItems.Clear();
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return order.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<OrderResponseDto>> GetClientOrdersAsync(int clientId)
        {
            var orders = await _db.Orders
            .Where(o => o.ClientId == clientId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToListAsync();

            if (!orders.Any())
                throw new KeyNotFoundException("Заказы не найдены");

            return orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Address = o.Address,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.Product.Id,
                    Name = oi.Product.Name,
                    Price = oi.Price * oi.Quantity,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();
        }
    }
}