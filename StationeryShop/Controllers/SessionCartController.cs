using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationeryShop.DTOs.Cart;
using StationeryShop.Services;
using StationeryShop.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using StationeryShop.Models;

namespace StationeryShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionCartController : ControllerBase
    {
        private readonly SessionCartService _sessionCartService;
        private readonly StationeryDbContext _db;

        public SessionCartController(SessionCartService cartService, StationeryDbContext db)
        {
            _sessionCartService = cartService;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var cart = await _sessionCartService.GetCartAsync();
                return Ok(cart);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при получении сессионной корзины");
            }
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemDto itemDto)
        {
            try
            {
                await _sessionCartService.AddToCartAsync(itemDto);
                return Ok("Товар добавлен в сессионную корзину");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при добавлении товара в корзину");
            }
        }

        //полностью удалить товар из корзины
        [HttpDelete("items/{productId}")]
        public IActionResult RemoveFromCart(int productId)
        {
            try
            {
                _sessionCartService.RemoveFromCart(productId);
                return Ok("Товар удален из сессионной корзины");
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при удалении товара из корзины");
            }
        }

        //уменьшить количество товара на 1
        [HttpPut("items/{productId}/decrease")]
        public async Task<IActionResult> DecreaseQuantity(int productId)
        {
            try
            {
                var result = _sessionCartService.DecreaseQuantity(productId);
                return Ok(new
                {
                    Removed = result.Removed,
                    Message = result.Removed
                        ? "Товар удален из корзины"
                        : $"Количество уменьшено до {result.NewQuantity}"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
