using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using RestFullAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add CORS để cho phép MVC_ADMIN gọi API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMVCAdmin", policy =>
    {
        policy.WithOrigins(
                "https://localhost:44319", 
                "http://localhost:44319",
                "http://localhost:64761",
                "https://localhost:64761"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowMVCAdmin");

app.UseAuthorization();

// Map API controllers
app.MapControllers();

// Root endpoint - trả về thông tin API
app.MapGet("/", () => new
{
    message = "REST API - Nền Tảng Học Liệu",
    version = "1.0",
    endpoints = new
    {
        swagger = "/swagger",
        api = "/api"
    },
    status = "running"
});

// Redirect /api to Swagger nếu cần
app.MapGet("/api", () => Results.Redirect("/swagger"));

app.Run();
