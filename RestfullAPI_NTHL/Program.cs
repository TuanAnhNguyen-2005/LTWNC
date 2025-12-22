using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RestfullAPI_NTHL.Models;
using Microsoft.AspNetCore.Authentication.Cookies; // THÊM ĐỂ DÙNG COOKIE AUTHENTICATION

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<NenTangDbContext>(options =>
{
    var connection = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connection);
});

// THÊM PHẦN NÀY: CẤU HÌNH AUTHENTICATION VỚI COOKIE
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đường dẫn trang login nếu chưa đăng nhập
        options.AccessDeniedPath = "/Account/AccessDenied"; // Đường dẫn khi không đủ quyền
        options.ExpireTimeSpan = TimeSpan.FromHours(8); // Thời gian hết hạn cookie
        options.SlidingExpiration = true; // Tự động gia hạn nếu còn hoạt động
    });

// THÊM PHẦN NÀY: AUTHORIZATION (nếu chưa có)
builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "NenTangHocLieu API", Version = "v1" });
});

var app = builder.Build();

// 🔴 FIX CHÍNH: CẤU HÌNH STATIC FILES EXPLICIT VÀ ĐẶT LÊN ĐẦU
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = ""
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// THÊM PHẦN NÀY: USE AUTHENTICATION (PHẢI TRƯỚC UseAuthorization)
app.UseAuthentication();

// ĐÃ CÓ: USE AUTHORIZATION
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NenTangHocLieu API v1");
    c.RoutePrefix = "swagger";
});

app.MapControllers();

app.Run();