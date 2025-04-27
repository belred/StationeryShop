using Microsoft.AspNetCore.Mvc;
using StationeryShop.Services;
using System.Security.Claims;
using StationeryShop.DTOs.Cart;
using Microsoft.AspNetCore.Authorization;

namespace StationeryShop.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var cart = await _cartService.GetCartAsync(clientId);
                return Ok(cart);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при получении корзины");
            }
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemDto itemDto)
        {
            try
            {
                var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _cartService.AddToCartAsync(clientId, itemDto);
                return Ok("Товар добавлен в корзину");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //удалить полностью товар из корзины
        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            try
            {
                var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _cartService.RemoveFromCartAsync(clientId, productId);
                return Ok("Товар удален из корзины");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        //уменьшить количество товара в корзине на 1
        [HttpPut("items/{productId}/decrease")]
        public async Task<IActionResult> DecreaseQuantity(int productId)
        {
            try
            {
                var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _cartService.DecreaseQuantityAsync(clientId, productId);

                return Ok(new
                {
                    message = result.Removed
                        ? $"Товар удален из корзины (было: {result.OldQuantity})"
                        : $"Количество уменьшено с {result.OldQuantity} до {result.NewQuantity}"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}