using EcommerceAPI.Data;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add services
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ✅ Register custom services
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// ✅ Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(
                    jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT secret key not configured")
                )
            ),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ✅ CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthentication();   // ✅ Add authentication middleware
app.UseAuthorization();

app.MapControllers();

// ✅ Seed demo data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
    
    // Ensure database is created
    db.Database.EnsureCreated();
    
    // Check if demo users already exist
    if (!db.Users.Any(u => u.Username == "admin"))
    {
        // Create admin user
        var adminUser = new EcommerceAPI.Models.User
        {
            Username = "admin",
            Email = "admin@ecommerce.com",
            Password = passwordService.HashPassword("password123"),
            FirstName = "Admin",
            LastName = "User",
            Role = "Admin",
            PhoneNumber = "555-0001"
        };
        db.Users.Add(adminUser);
        db.SaveChanges();
    }
    
    if (!db.Users.Any(u => u.Username == "customer"))
    {
        // Create customer user
        var customerUser = new EcommerceAPI.Models.User
        {
            Username = "customer",
            Email = "customer@ecommerce.com",
            Password = passwordService.HashPassword("password123"),
            FirstName = "John",
            LastName = "Doe",
            Role = "Customer",
            PhoneNumber = "555-0002"
        };
        db.Users.Add(customerUser);
        db.SaveChanges();
    }
    
    // Seed demo products if none exist
    if (!db.Products.Any())
    {
        var products = new[]
        {
            new EcommerceAPI.Models.Product { Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, Stock = 5, Category = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?auto=format&fit=crop&w=900&q=80", SKU = "LAPTOP-001", IsActive = true },
            new EcommerceAPI.Models.Product { Name = "Smartphone", Description = "Latest smartphone model", Price = 699.99m, Stock = 10, Category = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?auto=format&fit=crop&w=900&q=80", SKU = "PHONE-001", IsActive = true },
            new EcommerceAPI.Models.Product { Name = "Headphones", Description = "Wireless headphones", Price = 149.99m, Stock = 20, Category = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=900&q=80", SKU = "HEAD-001", IsActive = true },
            new EcommerceAPI.Models.Product { Name = "Tablet", Description = "10-inch tablet", Price = 449.99m, Stock = 8, Category = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1561154464-82e9adf32764?auto=format&fit=crop&w=900&q=80", SKU = "TAB-001", IsActive = true },
            new EcommerceAPI.Models.Product { Name = "Monitor", Description = "4K monitor", Price = 399.99m, Stock = 12, Category = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?auto=format&fit=crop&w=900&q=80", SKU = "MON-001", IsActive = true },
            new EcommerceAPI.Models.Product { Name = "Keyboard", Description = "Mechanical keyboard", Price = 129.99m, Stock = 15, Category = "Accessories", ImageUrl = "https://images.unsplash.com/photo-1511467687858-23d96c32e4ae?auto=format&fit=crop&w=900&q=80", SKU = "KEY-001", IsActive = true },
            new EcommerceAPI.Models.Product { Name = "Mouse", Description = "Ergonomic mouse", Price = 49.99m, Stock = 25, Category = "Accessories", ImageUrl = "https://images.unsplash.com/photo-1527814050087-3793815479db?auto=format&fit=crop&w=900&q=80", SKU = "MOUSE-001", IsActive = true },
            new EcommerceAPI.Models.Product { Name = "USB Cable", Description = "USB-C charging cable", Price = 19.99m, Stock = 50, Category = "Accessories", ImageUrl = "https://images.unsplash.com/photo-1580906855283-5f6f1c5f517f?auto=format&fit=crop&w=900&q=80", SKU = "CABLE-001", IsActive = true },
            new EcommerceAPI.Models.Product { Name = "Webcam", Description = "1080p HD webcam", Price = 89.99m, Stock = 10, Category = "Electronics", ImageUrl = "https://images.unsplash.com/photo-1587825140708-dfaf72ae4b04?auto=format&fit=crop&w=900&q=80", SKU = "WEB-001", IsActive = true },
            new EcommerceAPI.Models.Product { Name = "External SSD", Description = "1TB external SSD", Price = 119.99m, Stock = 18, Category = "Storage", ImageUrl = "https://images.unsplash.com/photo-1597872200969-2b65d56bd16b?auto=format&fit=crop&w=900&q=80", SKU = "SSD-001", IsActive = true }
        };
        
        foreach (var product in products)
        {
            db.Products.Add(product);
        }
        db.SaveChanges();
    }
    
    // Seed demo categories if none exist
    if (!db.Categories.Any())
    {
        var categories = new[]
        {
            new EcommerceAPI.Models.Category { Name = "Electronics", Description = "Electronic devices and gadgets", DisplayOrder = 1, IsActive = true },
            new EcommerceAPI.Models.Category { Name = "Accessories", Description = "Computer and device accessories", DisplayOrder = 2, IsActive = true },
            new EcommerceAPI.Models.Category { Name = "Storage", Description = "Storage devices", DisplayOrder = 3, IsActive = true }
        };
        
        foreach (var category in categories)
        {
            db.Categories.Add(category);
        }
        db.SaveChanges();
    }

    // Upgrade old placeholder image URLs to real pictures for existing seeded products.
    var productImageMap = new Dictionary<string, string>
    {
        ["LAPTOP-001"] = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?auto=format&fit=crop&w=900&q=80",
        ["PHONE-001"] = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?auto=format&fit=crop&w=900&q=80",
        ["HEAD-001"] = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=900&q=80",
        ["TAB-001"] = "https://images.unsplash.com/photo-1561154464-82e9adf32764?auto=format&fit=crop&w=900&q=80",
        ["MON-001"] = "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?auto=format&fit=crop&w=900&q=80",
        ["KEY-001"] = "https://images.unsplash.com/photo-1511467687858-23d96c32e4ae?auto=format&fit=crop&w=900&q=80",
        ["MOUSE-001"] = "https://images.unsplash.com/photo-1527814050087-3793815479db?auto=format&fit=crop&w=900&q=80",
        ["CABLE-001"] = "https://images.unsplash.com/photo-1580906855283-5f6f1c5f517f?auto=format&fit=crop&w=900&q=80",
        ["WEB-001"] = "https://images.unsplash.com/photo-1587825140708-dfaf72ae4b04?auto=format&fit=crop&w=900&q=80",
        ["SSD-001"] = "https://images.unsplash.com/photo-1597872200969-2b65d56bd16b?auto=format&fit=crop&w=900&q=80"
    };

    var needsImageUpgrade = db.Products
        .Where(p => p.ImageUrl != null && p.ImageUrl.Contains("via.placeholder.com"))
        .ToList();

    if (needsImageUpgrade.Any())
    {
        foreach (var product in needsImageUpgrade)
        {
            if (productImageMap.TryGetValue(product.SKU, out var newImageUrl))
            {
                product.ImageUrl = newImageUrl;
            }
        }

        db.SaveChanges();
    }
}

app.Run();