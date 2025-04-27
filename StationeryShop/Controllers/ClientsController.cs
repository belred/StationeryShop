using Microsoft.AspNetCore.Mvc;
using StationeryShop.Services;
using StationeryShop.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace StationeryShop.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ClientService _clientService;

        public ClientsController(ClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var client = await _clientService.GetClientAsync(userId);
                return Ok(client);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] ClientUpdateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _clientService.UpdateClientAsync(userId, dto);
                return Ok("Данные успешно обновлены");
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

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteCurrentUser()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _clientService.DeleteClientAsync(userId);
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); //выйти из аккаунта
                return Ok("Аккаунт успешно удален");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}