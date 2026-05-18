using Microsoft.EntityFrameworkCore;
using ShoesShop.API.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

// Lấy Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Cấu hình sử dụng SQL Server thay vì MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Thêm Controllers, Swagger, v.v...
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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