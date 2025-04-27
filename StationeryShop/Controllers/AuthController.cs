using Microsoft.AspNetCore.Mvc;
using StationeryShop.Services;
using StationeryShop.DTOs.Auth;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace StationeryShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                    return BadRequest(new { error = "Сначала выйдите из текущего аккаунта" });

                var result = await _authService.RegisterAsync(dto);
                if (!result.Success_)
                    return BadRequest(new { error = result.Error });

                //сохраняем ID в сессии
                HttpContext.Session.SetInt32("UserId", (int)result.UserId);

                return Ok(new { message = "Регистрация успешно пройдена" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ошибка сервера" });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                    return BadRequest(new { error = "Вы уже авторизованы" });

                var result = await _authService.LoginAsync(dto.Email, dto.Password);
                if (!result.Success_)
                {
                    if (result.Error == "Аккаунт был удален. Восстановите его через регистрацию")
                        return Unauthorized(new
                        {
                            error = result.Error,
                            solution = "Попробуйте зарегистрироваться снова с тем же email"
                        });

                    return Unauthorized(new { error = result.Error });
                }

                var claims = new List<Claim>
                {
                new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                new Claim(ClaimTypes.Email, dto.Email),
                new Claim(ClaimTypes.Name, result.Name)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddHours(2)
                    });

                return Ok(new { message = "Вы успешно вошли в аккаунт" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ошибка сервера" });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok(new { message = "Вы успешно вышли из аккаунта" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ошибка при выходе" });
            }
        }
    }
}