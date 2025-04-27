namespace StationeryShop.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        public Client Client { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
