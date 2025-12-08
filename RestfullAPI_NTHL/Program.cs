// Program.cs – RESTfulAPI.NTHL (ASP.NET Core)
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// BẬT SWAGGER – 2 DÒNG QUAN TRỌNG
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "NenTangHocLieu API", Version = "v1" });
});

var app = builder.Build();

// BẬT SWAGGER TRONG MỌI MÔI TRƯỜNG (Development + Production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NenTangHocLieu API v1");
    c.RoutePrefix = "swagger"; // vào https://localhost:7068/swagger
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();