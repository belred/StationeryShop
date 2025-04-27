using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StationeryShop.DTOs.Review;
using StationeryShop.Services;
using System.Security.Claims;

namespace StationeryShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewsController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview([FromBody] ReviewCreateDto reviewDto)
        {
            try
            {
                var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _reviewService.AddReviewAsync(clientId, reviewDto);
                return Ok("Отзыв успешно добавлен");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); //на товар один пользователь может оставить только один отзыв
            }
        }

        //получить отзывы о товаре по id товара
        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            try
            {
                if (productId <= 0)
                    throw new ArgumentException("ID товара должен быть положительным числом");

                var reviews = await _reviewService.GetProductReviewsAsync(productId);
                return Ok(reviews);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла внутренняя ошибка сервера");
            }
        }
    }
}