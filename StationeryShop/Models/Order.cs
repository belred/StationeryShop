using System.ComponentModel.DataAnnotations.Schema;

namespace StationeryShop.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; //статус (Pending, Shipped, Delivered)
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public string Address { get; set; }


        public Client Client { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
