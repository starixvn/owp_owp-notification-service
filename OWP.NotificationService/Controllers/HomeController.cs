using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Models.ParameterObject;
using Fanex.NAP.Client.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OWP.NotificationService.Controllers;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly INapSession session;

    public HomeController(INapSession session)
    {
        this.session = session;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Title = "OWP Notification Service",
            IsSessionAvailable = HttpContext.Session.IsAvailable,
            UserId = session.UserId,
            Username = session.UserName
        });
    }

    [HttpGet("FakeLogin")]
    public async Task<IActionResult> FakeLogin()
    {
        var criteria = new LoginParameter
        {
            UserId = 8,
            SiteId = 99,
            LoginDate = DateTime.Now,
            NetSessionId = HttpContext?.Session.Id,
            Ip = "127.0.0.1",
            BrowserName = Request?.Headers["User-Agent"].ToString(),
            CountryCode = "VN",
            Domain = "owp-local.com",
            Platform = Environment.OSVersion.Platform.ToString()
        };

        var securityService = new SecurityService();
        var onUserId = securityService.Login(criteria);

        await SaveUserSession(onUserId);

        return Ok(new
        {
            Title = "OWP Notification Service Faked Login",
            IsSessionAvailable = HttpContext!.Session.IsAvailable,
            UserId = session.UserId,
            Username = session.UserName
        });
    }

    private async Task SaveUserSession(int onUserId)
    {
        const int fakeUserId = 8;
        session.UserId = fakeUserId;
        session.UserName = "sa";
        session.OnUserId = onUserId;

        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, "sa"),
                            new Claim(ClaimTypes.NameIdentifier, "8")
                        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));


    }

    [HttpGet("ExpiredSession")]
    public string ExpiredSession()
    {
        return "ExpiredSession";
    }
}
