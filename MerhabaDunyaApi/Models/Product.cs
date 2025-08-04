using System;

namespace MerhabaDunyaApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; }
        public decimal Price { get; set; }
        public double Weight { get; set; }
        public string ImageUrl { get; set; }
        public double Carbon { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}