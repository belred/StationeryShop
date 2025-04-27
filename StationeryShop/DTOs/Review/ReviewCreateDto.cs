using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StationeryShop.DTOs.Review
{
    public class ReviewCreateDto
    {
        [Required(ErrorMessage = "ID товара обязательно")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Оценка обязательна")]
        [Range(1, 5, ErrorMessage = "Оценка от 1 до 5")]
        [DefaultValue("5")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Максимум 1000 символов")]
        [DefaultValue("text")]
        public string? Comment { get; set; }
    }
}