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
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> PlaceOrder(int userId, [FromBody] PlaceOrderRequest request)
        {
            try
            {
                // ✅ Verify authorization
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (currentUserId != userId && !User.IsInRole("Admin"))
                    return Forbid();

                var cartItems = await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .Include(c => c.Product)
                    .ToListAsync();

                if (!cartItems.Any())
                    return BadRequest(new { message = "Cart is empty" });

                // ✅ Create order
                decimal totalAmount = 0;
                var order = new Order
                {
                    UserId = userId,
                    Status = "Pending",
                    ShippingAddress = request.ShippingAddress
                };

                foreach (var cartItem in cartItems)
                {
                    var product = cartItem.Product;
                    if (product == null)
                        return BadRequest(new { message = "Product not found in cart" });

                    if (product.Stock < cartItem.Quantity)
                        return BadRequest(new { message = $"Insufficient stock for {product.Name}" });

                    // ✅ Create order item
                    var orderItem = new OrderItem
                    {
                        Order = order,
                        ProductId = product.Id,
                        Quantity = cartItem.Quantity,
                        Price = product.Price,
                        Discount = product.Discount
                    };

                    totalAmount += (product.Price * cartItem.Quantity) - ((product.Discount ?? 0) * cartItem.Quantity);
                    order.OrderItems.Add(orderItem);

                    // ✅ Update stock
                    product.Stock -= cartItem.Quantity;
                }

                order.TotalAmount = totalAmount;

                // ✅ Create payment record
                var payment = new Payment
                {
                    Order = order,
                    Amount = totalAmount,
                    Status = "Pending",
                    PaymentMethod = request.PaymentMethod ?? "Credit Card"
                };
                order.Payment = payment;

                _context.Orders.Add(order);
                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, new
                {
                    orderId = order.Id,
                    totalAmount = order.TotalAmount,
                    status = order.Status,
                    paymentRequired = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error placing order", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .Include(o => o.Payment)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                    return NotFound(new { message = "Order not found" });

                // ✅ Verify authorization
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (currentUserId != order.UserId && !User.IsInRole("Admin"))
                    return Forbid();

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving order", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            try
            {
                // ✅ Verify authorization
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (currentUserId != userId && !User.IsInRole("Admin"))
                    return Forbid();

                var orders = await _context.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving orders", error = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                    return NotFound(new { message = "Order not found" });

                var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
                if (!validStatuses.Contains(request.Status))
                    return BadRequest(new { message = "Invalid order status" });

                order.Status = request.Status;
                if (!string.IsNullOrWhiteSpace(request.TrackingNumber))
                    order.TrackingNumber = request.TrackingNumber;

                if (request.Status == "Delivered" && !order.DeliveredAt.HasValue)
                    order.DeliveredAt = DateTime.UtcNow;

                order.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Order status updated successfully", order });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating order status", error = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] string? status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = _context.Orders.AsQueryable();

                if (!string.IsNullOrWhiteSpace(status))
                    query = query.Where(o => o.Status == status);

                var totalCount = await query.CountAsync();
                var orders = await query
                    .OrderByDescending(o => o.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ToListAsync();

                return Ok(new
                {
                    totalCount,
                    pageNumber,
                    pageSize,
                    orders
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving orders", error = ex.Message });
            }
        }
    }

    // ✅ DTOs
    public class PlaceOrderRequest
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? TrackingNumber { get; set; }
    }
}