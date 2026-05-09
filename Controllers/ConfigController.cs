using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CompanyDashboardAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly IConfiguration _config;

    public ConfigController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet]
    public IActionResult GetConfig()
    {
        return Ok(new
        {
            GOOGLE_CLIENT_ID = _config["Google:ClientId"] ?? "",
            API_URL = _config["ApiUrl"] ?? "http://localhost:5200",
            MAINTENANCE_MODE = _config.GetValue<bool>("MaintenanceMode")
        });
    }
}
