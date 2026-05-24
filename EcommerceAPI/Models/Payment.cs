using System;

namespace EcommerceAPI.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded

        public string PaymentMethod { get; set; } = "Credit Card"; // Credit Card, PayPal, etc.

        public string? StripeTransactionId { get; set; }

        public string? CardLastFour { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedAt { get; set; }

        // Navigation properties
        public Order? Order { get; set; }
    }
}
