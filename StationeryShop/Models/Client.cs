using System.ComponentModel.DataAnnotations;

namespace StationeryShop.Models
{
    public class Client
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public string Phone { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedData { get; set; }


        public Cart Cart { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
