﻿namespace StationeryShop.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }


        public Client Client { get; set; }
        public Product Product { get; set; }
    }
}
