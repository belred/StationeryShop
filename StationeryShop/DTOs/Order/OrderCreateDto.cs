using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StationeryShop.DTOs.Order
{
    public class OrderCreateDto
    {
        [Required(ErrorMessage = "Адрес обязателен")]
        [StringLength(200, ErrorMessage = "Адрес слишком длинный")]
        [RegularExpression(
            @"^[а-яА-ЯёЁ\s-]+\s[а-яА-ЯёЁ\s-]+,\s\d+[а-яА-Я]?\sкв\.\s\d+$",
            ErrorMessage = "Формат: Город Улица, Дом кв. Номер")]
        [DefaultValue("Москва Тверская, 15 кв. 7")]
        public string Address { get; set; }
    }
}