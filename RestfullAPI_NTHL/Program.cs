// Program.cs – RESTfulAPI_NTHL
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RestfullAPI_NTHL.Models;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<NenTangDbContext>(options =>
{
    var connection = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connection);
});

// ✅ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Controllers + JSON tránh vòng tham chiếu
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

app.UseHttpsRedirection();

// ✅ Quan trọng: UseCors trước MapControllers (và trước Authorization càng tốt)
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();
