using System;

namespace EcommerceAPI.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public Order? Order { get; set; }

        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; } // Price at time of purchase

        public decimal? Discount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}