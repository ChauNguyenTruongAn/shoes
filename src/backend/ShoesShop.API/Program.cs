using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ShoesShop.API.Features.Categories;
using ShoesShop.API.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using ShoesShop.API.Features.Brands;

var builder = WebApplication.CreateBuilder(args);

// Lấy Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Cấu hình sử dụng SQL Server thay vì MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();

// Thêm Controllers, Swagger, v.v...
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();