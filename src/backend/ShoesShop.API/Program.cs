using Microsoft.EntityFrameworkCore;
using ShoesShop.API.Features.Categories;
using ShoesShop.API.Infrastructure.Database;
using ShoesShop.API.Features.Brands;
using ShoesShop.API.Features.Products;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Lấy Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Cấu hình sử dụng SQL Server thay vì MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ShoesShop.API.Infrastructure.Files.IFileService, ShoesShop.API.Infrastructure.Files.FileService>();

builder.Services.AddScoped<ShoesShop.API.Features.Auth.IAuthService, ShoesShop.API.Features.Auth.AuthService>();
builder.Services.AddScoped<ShoesShop.API.Features.Cart.ICartService, ShoesShop.API.Features.Cart.CartService>();
builder.Services.AddScoped<ShoesShop.API.Features.Addresses.IAddressService, ShoesShop.API.Features.Addresses.AddressService>();
builder.Services.AddScoped<ShoesShop.API.Features.Orders.IOrderService, ShoesShop.API.Features.Orders.OrderService>();
builder.Services.AddScoped<ShoesShop.API.Features.Promotions.IPromotionService, ShoesShop.API.Features.Promotions.PromotionService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ShoesShop.API.Features.Payments.IVnPayService, ShoesShop.API.Features.Payments.VnPayService>();

// Cấu hình JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
    };
});

// Cấu hình Swagger để nó hỗ trợ nút Authorize (Ổ khóa) cho JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShoesShop.API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập 'Bearer [khoảng trắng] {token của bạn}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Quét toàn bộ Assembly hiện tại để tự động tìm mọi file MappingProfile
builder.Services.AddAutoMapper(config => 
{
    config.AddMaps(System.Reflection.Assembly.GetExecutingAssembly());
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication(); // Xác thực xem user là ai (Đọc Token)
app.UseAuthorization();
app.MapControllers();
app.Run();