using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StationeryShop.DTOs.Order;
using StationeryShop.Services;
using System.Security.Claims;

namespace StationeryShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            try
            {
                var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var orderId = await _orderService.CreateOrderAsync(clientId, orderDto);
                return Ok(new { OrderId = orderId, message = "Заказ успешно оформлен" });
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
                return Conflict(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetClientOrders()
        {
            try
            {
                var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var orders = await _orderService.GetClientOrdersAsync(clientId);

                if (orders.Count == 0)
                    return Ok(new { Message = "У вас пока нет заказов" });

                return Ok(orders);
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