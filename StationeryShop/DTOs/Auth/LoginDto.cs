using System.ComponentModel.DataAnnotations;

namespace StationeryShop.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Минимум 6 символов")]
        public string Password { get; set; }
    }
}