using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StationeryShop.DTOs.Cart
{
    public class CartItemDto
    {
        [Required(ErrorMessage = "ID товара обязательно")]
        public int ProductId { get; set; }

        [Range(1, 1, ErrorMessage = "Можно добавить только 1 товар за раз")]
        [DefaultValue(1)]
        public int Quantity { get; set; } = 1;
    }
}