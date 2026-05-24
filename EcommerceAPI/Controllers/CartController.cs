using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            try
            {
                // ✅ Verify authorization - user can only see their own cart
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (currentUserId != userId && !User.IsInRole("Admin"))
                    return Forbid();

                var cartItems = await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .Include(c => c.Product)
                    .ToListAsync();

                var cartTotal = cartItems.Sum(c => (c.Product?.Price ?? 0) * c.Quantity);

                var responseItems = cartItems.Select(c => new
                {
                    id = c.Id,
                    userId = c.UserId,
                    productId = c.ProductId,
                    quantity = c.Quantity,
                    addedAt = c.AddedAt,
                    product = c.Product == null ? null : new
                    {
                        id = c.Product.Id,
                        name = c.Product.Name,
                        description = c.Product.Description,
                        price = c.Product.Price,
                        category = c.Product.Category,
                        imageUrl = c.Product.ImageUrl,
                        stock = c.Product.Stock,
                        sku = c.Product.SKU,
                        discount = c.Product.Discount,
                        isActive = c.Product.IsActive
                    }
                });

                return Ok(new
                {
                    cartItems = responseItems,
                    totalItems = cartItems.Count,
                    totalAmount = cartTotal
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving cart", error = ex.Message });
            }
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> AddToCart(int userId, [FromBody] AddToCartRequest request)
        {
            try
            {
                // ✅ Verify authorization
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (currentUserId != userId && !User.IsInRole("Admin"))
                    return Forbid();

                var product = await _context.Products.FindAsync(request.ProductId);
                if (product == null)
                    return NotFound(new { message = "Product not found" });

                if (product.Stock < request.Quantity)
                    return BadRequest(new { message = "Insufficient stock" });

                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == request.ProductId);

                if (existingItem != null)
                {
                    existingItem.Quantity += request.Quantity;
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        UserId = userId,
                        ProductId = request.ProductId,
                        Quantity = request.Quantity
                    };
                    _context.CartItems.Add(cartItem);
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Item added to cart successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding to cart", error = ex.Message });
            }
        }

        [HttpPatch("{userId}/{productId}")]
        public async Task<IActionResult> UpdateCartItem(int userId, int productId, [FromBody] UpdateCartRequest request)
        {
            try
            {
                // ✅ Verify authorization
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (currentUserId != userId && !User.IsInRole("Admin"))
                    return Forbid();

                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

                if (cartItem == null)
                    return NotFound(new { message = "Cart item not found" });

                if (request.Quantity <= 0)
                    return BadRequest(new { message = "Quantity must be greater than 0" });

                var product = await _context.Products.FindAsync(productId);
                if (product!.Stock < request.Quantity)
                    return BadRequest(new { message = "Insufficient stock" });

                cartItem.Quantity = request.Quantity;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Cart item updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating cart item", error = ex.Message });
            }
        }

        [HttpDelete("{userId}/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int userId, int productId)
        {
            try
            {
                // ✅ Verify authorization
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (currentUserId != userId && !User.IsInRole("Admin"))
                    return Forbid();

                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

                if (cartItem == null)
                    return NotFound(new { message = "Cart item not found" });

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Item removed from cart" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error removing from cart", error = ex.Message });
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            try
            {
                // ✅ Verify authorization
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (currentUserId != userId && !User.IsInRole("Admin"))
                    return Forbid();

                var cartItems = await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Cart cleared successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error clearing cart", error = ex.Message });
            }
        }
    }

    // ✅ DTOs
    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartRequest
    {
        public int Quantity { get; set; }
    }
}