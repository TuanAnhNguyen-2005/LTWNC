using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RestfullAPI_NTHL.Models;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<NenTangDbContext>(options =>
{
    var connection = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connection);
});

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

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NenTangHocLieu API v1");
    c.RoutePrefix = "swagger";
});

// 🔴 FIX CHÍNH: CẤU HÌNH STATIC FILES EXPLICIT VÀ ĐẶT LÊN ĐẦU
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = ""
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Nếu bạn có JWT authentication
// app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();