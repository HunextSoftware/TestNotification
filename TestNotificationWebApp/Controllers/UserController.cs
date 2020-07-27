using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestNotificationWebApp.Models;

//IS IT NEEDED???
namespace TestNotificationWebApp.Controllers
{
    public class UserController : Controller
    {
        readonly NotificationHubClient _hub;

        public UserController(IOptions<NotificationHubOptions> options)
        {
            _hub = NotificationHubClient.CreateClientFromConnectionString(
                options.Value.ConnectionString,
                options.Value.Name);
        }

        [HttpGet]
        public async Task<ActionResult> ListRegisteredDevice()
        {
            var registrations = new List<RegistrationDescription>(await _hub.GetAllRegistrationsAsync(0, CancellationToken.None));
            return View("Information", registrations);
        }
    }
}
