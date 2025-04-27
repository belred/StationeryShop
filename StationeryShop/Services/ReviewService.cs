using StationeryShop.Models;
using StationeryShop.Data;
using StationeryShop.DTOs.Review;
using Microsoft.EntityFrameworkCore;

namespace StationeryShop.Services
{
    public class ReviewService
    {
        private readonly StationeryDbContext _db;

        public ReviewService(StationeryDbContext db)
        {
            _db = db;
        }

        public async Task AddReviewAsync(int clientId, ReviewCreateDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Рейтинг должен быть от 1 до 5");

            var productExists = await ProductExistsAsync(dto.ProductId);
            if (!productExists)
                throw new KeyNotFoundException("Товар не найден");

            var alreadyReviewed = await _db.Reviews
                .AnyAsync(r => r.ClientId == clientId && r.ProductId == dto.ProductId);

            if (alreadyReviewed)
                throw new InvalidOperationException("Вы уже оставляли отзыв на этот товар");

            var review = new Review
            {
                ClientId = clientId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await _db.Reviews.AddAsync(review);
            await _db.SaveChangesAsync();
        }

        private async Task<bool> ProductExistsAsync(int productId)
        {
            return await _db.Products.FindAsync(productId) != null;
        }

        public async Task<List<ReviewResponseDto>> GetProductReviewsAsync(int productId)
        {
            if (!await ProductExistsAsync(productId))
                throw new KeyNotFoundException("Товар не найден");

            var reviews = await _db.Reviews
                .Where(r => r.ProductId == productId)
                .Include(r => r.Client)
                .Select(r => new ReviewResponseDto
                {
                    Rating = r.Rating,
                    Comment = r.Comment,
                    Author = new ReviewResponseDto.ReviewerDto
                    {
                        Id = r.Client.Id,
                        Name = r.Client.Name
                    }
                })
                .ToListAsync();

            if (reviews.Count == 0)
                throw new KeyNotFoundException("Отзывов к этому товару ещё нет");

            return reviews;
        }
    }
}