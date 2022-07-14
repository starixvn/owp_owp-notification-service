using Fanex.NAP.Client.Helpers.Web;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OWP.Core.NotificationRequests;

namespace OWP.NotificationService.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly INapSession napSession;
        private readonly IMediator mediator;

        public TestController(INapSession napSession, IMediator mediator)
        {
            this.napSession = napSession;
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var request = new NewTicketNotificationRequest();
            await mediator.Send(request);
            return Ok();
        }
    }
}
