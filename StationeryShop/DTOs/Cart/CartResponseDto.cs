namespace StationeryShop.DTOs.Cart
{
    public class CartResponseDto
    {
        public List<CartItemDetailDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }

        public class CartItemDetailDto
        {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }
    }
}