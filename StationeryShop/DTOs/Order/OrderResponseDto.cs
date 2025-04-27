using System.Text.Json.Serialization;

namespace StationeryShop.DTOs.Order
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime OrderDate { get; set; }
        public string Address { get; set; }
        public string FormattedOrderDate => OrderDate.ToString("dd.MM.yyyy HH:mm");
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();

        public class OrderItemDto
        {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }
    }
}