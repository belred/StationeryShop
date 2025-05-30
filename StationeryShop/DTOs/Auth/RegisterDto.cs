﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StationeryShop.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Длина имени от 2 до 100 символов")]
        [DefaultValue("")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Минимум 6 символов")]
        [DefaultValue("******")]
        public string Password { get; set; }
        [RegularExpression(
            @"^\+?[0-9]{1,3}?[-\s]?\(?[0-9]{3}\)?[-\s]?[0-9]{3}[-\s]?[0-9]{2}[-\s]?[0-9]{2}$",
            ErrorMessage = "Формат: +7 (XXX) XXX-XX-XX или XXX-XXX-XX-XX")]
        [DefaultValue("+7 (123) 456-78-90")]
        public string? Phone { get; set; }
    }
}