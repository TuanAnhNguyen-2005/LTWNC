// Controllers/ClientConfigController.cs
using Microsoft.AspNetCore.Mvc;

[Route("api/config")]
[ApiController]
public class ClientConfigController : ControllerBase
{
    private readonly IConfiguration _config;

    public ClientConfigController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet("urls")]
    public IActionResult GetUrls()
    {
        return Ok(new
        {
            ApiBaseUrl = _config["ApiSettings:BaseUrl"],
            ApiBaseUrlHttp = _config["ApiSettings:BaseUrlHttp"],
            AdminUrl = _config["ClientUrls:Admin"],
            StudentUrl = _config["ClientUrls:Student"],
            TeacherUrl = _config["ClientUrls:Teacher"]
        });
    }
}