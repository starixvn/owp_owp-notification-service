using Microsoft.AspNetCore.Mvc;

namespace OWP.NotificationService.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public string Get() => "OWP Notification";
}
