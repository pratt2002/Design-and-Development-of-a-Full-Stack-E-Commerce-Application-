using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Payment)
                    .FirstOrDefaultAsync(o => o.Id == request.OrderId);

                if (order == null)
                    return NotFound(new { message = "Order not found" });

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (currentUserId != order.UserId && !User.IsInRole("Admin"))
                    return Forbid();

                var payment = order.Payment;
                if (payment == null)
                    return BadRequest(new { message = "Payment not found for order" });

                if (payment.Status != "Pending")
                    return BadRequest(new { message = "Payment already processed" });

                // ✅ Mock Stripe payment processing
                if (!ValidatePaymentDetails(request))
                    return BadRequest(new { message = "Invalid payment details" });

                // Simulate payment processing
                var transactionId = GenerateTransactionId();

                payment.Status = "Completed";
                payment.StripeTransactionId = transactionId;
                payment.CardLastFour = request.CardNumber.Substring(request.CardNumber.Length - 4);
                payment.ProcessedAt = DateTime.UtcNow;

                order.Status = "Processing";
                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Payment processed successfully",
                    transactionId,
                    status = "Completed",
                    amount = payment.Amount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error processing payment", error = ex.Message });
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetPaymentStatus(int orderId)
        {
            try
            {
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
                if (payment == null)
                    return NotFound(new { message = "Payment not found" });

                var order = await _context.Orders.FindAsync(orderId);
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (currentUserId != order?.UserId && !User.IsInRole("Admin"))
                    return Forbid();

                return Ok(new
                {
                    orderId = payment.OrderId,
                    amount = payment.Amount,
                    status = payment.Status,
                    paymentMethod = payment.PaymentMethod,
                    cardLastFour = payment.CardLastFour,
                    processedAt = payment.ProcessedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving payment status", error = ex.Message });
            }
        }

        [HttpPost("refund")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RefundPayment([FromBody] RefundRequest request)
        {
            try
            {
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == request.OrderId);
                if (payment == null)
                    return NotFound(new { message = "Payment not found" });

                if (payment.Status != "Completed")
                    return BadRequest(new { message = "Only completed payments can be refunded" });

                payment.Status = "Refunded";
                payment.UpdatedAt = DateTime.UtcNow;

                var order = await _context.Orders.FindAsync(request.OrderId);
                if (order != null)
                {
                    order.Status = "Cancelled";
                    order.UpdatedAt = DateTime.UtcNow;

                    // ✅ Restore stock
                    var orderItems = await _context.OrderItems
                        .Where(oi => oi.OrderId == order.Id)
                        .Include(oi => oi.Product)
                        .ToListAsync();

                    foreach (var orderItem in orderItems)
                    {
                        if (orderItem.Product != null)
                            orderItem.Product.Stock += orderItem.Quantity;
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Payment refunded successfully",
                    amount = payment.Amount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error processing refund", error = ex.Message });
            }
        }

        private bool ValidatePaymentDetails(PaymentRequest request)
        {
            // ✅ Basic validation
            if (string.IsNullOrWhiteSpace(request.CardNumber) || 
                string.IsNullOrWhiteSpace(request.CardholderName) ||
                string.IsNullOrWhiteSpace(request.Expiry) ||
                string.IsNullOrWhiteSpace(request.CVV))
                return false;

            // Mock validation - in real app, use Stripe API
            if (request.CardNumber.Length < 13 || request.CardNumber.Length > 19)
                return false;

            if (request.CVV.Length < 3 || request.CVV.Length > 4)
                return false;

            return true;
        }

        private string GenerateTransactionId()
        {
            return "TXN_" + Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper();
        }
    }

    // ✅ DTOs
    public class PaymentRequest
    {
        public int OrderId { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public string CardholderName { get; set; } = string.Empty;
        public string Expiry { get; set; } = string.Empty;
        public string CVV { get; set; } = string.Empty;
    }

    public class RefundRequest
    {
        public int OrderId { get; set; }
        public string? Reason { get; set; }
    }
}
