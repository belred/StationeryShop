using StationeryShop.Models;
using StationeryShop.Data;
using StationeryShop.DTOs.Auth;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace StationeryShop.Services
{
    public class AuthService
    {
        private readonly StationeryDbContext _db;
        private readonly SessionCartService _sessionCartService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(StationeryDbContext db, SessionCartService sessionCartService, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _sessionCartService = sessionCartService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthResult> RegisterAsync(RegisterDto dto)
        {
            try
            {
                if (!Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    return AuthResult.Fail("Некорректный email");

                //проверяем существование удаленного аккаунта
                var existingClient = await _db.Clients
                    .FirstOrDefaultAsync(c => c.Email == dto.Email);

                if (existingClient != null)
                {
                    if (!existingClient.IsDeleted)
                        return AuthResult.Fail("Email уже занят");

                    if (dto.Phone != null && !Regex.IsMatch(dto.Phone, @"^\+?[0-9]{1,3}?[-\s]?\(?[0-9]{3}\)?[-\s]?[0-9]{3}[-\s]?[0-9]{2}[-\s]?[0-9]{2}$"))
                        return AuthResult.Fail("Формат: +7 (XXX) XXX-XX-XX или XXX-XXX-XX-XX");

                    //восстанавливаем аккаунт
                    existingClient.IsDeleted = false;
                    existingClient.DeletedData = null;
                    existingClient.Name = dto.Name.Trim();
                    existingClient.Password = SimplePasswordHasher.HashPassword(dto.Password);
                    existingClient.Phone = dto.Phone?.Trim();

                    await _db.SaveChangesAsync();

                    //явный выход из системы перед восстановлением
                    if (_httpContextAccessor.HttpContext?.User.Identity.IsAuthenticated == true)
                    {
                        await _httpContextAccessor.HttpContext.SignOutAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme);
                    }

                    return AuthResult.Success(existingClient.Id, "Аккаунт восстановлен");
                }

                if (dto.Phone != null && !Regex.IsMatch(dto.Phone, @"^\+?[0-9]{1,3}?[-\s]?\(?[0-9]{3}\)?[-\s]?[0-9]{3}[-\s]?[0-9]{2}[-\s]?[0-9]{2}$"))
                    return AuthResult.Fail("Формат: +7 (XXX) XXX-XX-XX или XXX-XXX-XX-XX");

                var client = new Client
                {
                    Name = dto.Name.Trim(),
                    Email = dto.Email.ToLower().Trim(),
                    Password = SimplePasswordHasher.HashPassword(dto.Password),
                    Phone = dto.Phone?.Trim()
                };

                await _db.Clients.AddAsync(client);
                await _db.SaveChangesAsync();

                await _db.Carts.AddAsync(new Cart { ClientId = client.Id });
                await _db.SaveChangesAsync();

                //переносим товары из сессионной корзины
                await _sessionCartService.MergeWithUserCartAsync(client.Id);

                return AuthResult.Success(client.Id, client.Name);
            }
            catch (Exception ex)
            {
                return AuthResult.Fail("Ошибка сервера");
            }
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            try
            {
                var client = await _db.Clients
                    .FirstOrDefaultAsync(c => c.Email == email.ToLower().Trim());

                if (client == null || !SimplePasswordHasher.VerifyPassword(client.Password, password))
                    return AuthResult.Fail("Неверный email или пароль");

                if (client.IsDeleted)
                {
                    //явный выход из системы, если вдруг был вход
                    if (_httpContextAccessor.HttpContext?.User.Identity.IsAuthenticated == true)
                    {
                        await _httpContextAccessor.HttpContext.SignOutAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme);
                    }

                    var deletedDate = client.DeletedData?.ToString("dd.MM.yyyy") ?? "неизвестно";
                    return AuthResult.Fail($"Аккаунт был удален {deletedDate}. Восстановите его через регистрацию");
                }

                //переносим товары из сессионной корзины
                await _sessionCartService.MergeWithUserCartAsync(client.Id);

                return AuthResult.Success(client.Id, client.Name);
            }
            catch (Exception ex)
            {
                return AuthResult.Fail("Ошибка сервера");
            }
        }
    }

    public class AuthResult
    {
        public bool Success_ { get; }
        public string Error { get; }
        public int UserId { get; }
        public string Name { get; }

        private AuthResult(bool success, string error, int userId, string name)
        {
            Success_ = success;
            Error = error;
            UserId = userId;
            Name = name;
        }

        public static AuthResult Success(int userId, string name) =>
            new AuthResult(true, null, userId, name);

        public static AuthResult Fail(string error) =>
            new AuthResult(false, error, 0, null);
    }
}