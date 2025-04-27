using StationeryShop.Models;
using StationeryShop.DTOs.Product;
using Microsoft.EntityFrameworkCore;

namespace StationeryShop.Services
{
    public class ProductService
    {
        private readonly StationeryDbContext _db;

        public ProductService(StationeryDbContext db)
        {
            _db = db;
        }

        public async Task<List<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _db.Products
                .AsNoTracking()
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price
                })
                .ToListAsync();

            if (products.Count == 0)
            {
                throw new KeyNotFoundException("В базе данных нет товаров");
            }

            return products;
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID товара должен быть положительным числом");

            var product = await _db.Products.FindAsync(id);

            if (product == null)
                throw new KeyNotFoundException($"Товар с ID {id} не найден");

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price
            };
        }

        public async Task<List<ProductResponseDto>> SearchProductsAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name.Trim()))
                throw new ArgumentException("Название для поиска не может быть пустым");

            var products = await _db.Products
                .Where(p => p.Name.Contains(name))
                .ToListAsync();

            if (products.Count == 0)
                throw new KeyNotFoundException($"Товары по запросу '{name}' не найдены");

            return products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price
            }).ToList();
        }
    }
}