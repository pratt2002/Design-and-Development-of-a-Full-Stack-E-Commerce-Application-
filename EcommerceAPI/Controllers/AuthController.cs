using Microsoft.AspNetCore.Mvc;
using System.Linq;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using EcommerceAPI.Services;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;

        public AuthController(AppDbContext context, IPasswordService passwordService, IJwtService jwtService)
        {
            _context = context;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
                    return BadRequest(new { message = "Username and password are required" });

                var user = _context.Users
                    .FirstOrDefault(u => u.Username == loginRequest.Username);

                if (user == null || !_passwordService.VerifyPassword(loginRequest.Password, user.Password))
                    return Unauthorized(new { message = "Invalid username or password" });

                var token = _jwtService.GenerateToken(user.Id, user.Username, user.Role);

                return Ok(new
                {
                    token,
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role,
                    firstName = user.FirstName,
                    lastName = user.LastName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                // ✅ Validation
                if (string.IsNullOrWhiteSpace(registerRequest.Username) || 
                    string.IsNullOrWhiteSpace(registerRequest.Email) || 
                    string.IsNullOrWhiteSpace(registerRequest.Password))
                    return BadRequest(new { message = "Username, email, and password are required" });

                if (registerRequest.Password.Length < 8)
                    return BadRequest(new { message = "Password must be at least 8 characters long" });

                // ✅ Check if user already exists
                if (_context.Users.Any(u => u.Username == registerRequest.Username))
                    return BadRequest(new { message = "Username already exists" });

                if (_context.Users.Any(u => u.Email == registerRequest.Email))
                    return BadRequest(new { message = "Email already exists" });

                // ✅ Create new user
                var user = new User
                {
                    Username = registerRequest.Username,
                    Email = registerRequest.Email,
                    Password = _passwordService.HashPassword(registerRequest.Password),
                    FirstName = registerRequest.FirstName,
                    LastName = registerRequest.LastName,
                    Role = "Customer"
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                var token = _jwtService.GenerateToken(user.Id, user.Username, user.Role);

                return Ok(new
                {
                    token,
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role,
                    message = "User registered successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
            }
        }

        [HttpPost("verify-token")]
        public IActionResult VerifyToken([FromHeader] string authorization)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(authorization) || !authorization.StartsWith("Bearer "))
                    return Unauthorized(new { message = "Invalid token format" });

                var token = authorization.Substring("Bearer ".Length).Trim();
                var principal = _jwtService.ValidateToken(token);

                if (principal == null)
                    return Unauthorized(new { message = "Invalid or expired token" });

                return Ok(new { message = "Token is valid", principal });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }

    // ✅ DTOs for request bodies
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}