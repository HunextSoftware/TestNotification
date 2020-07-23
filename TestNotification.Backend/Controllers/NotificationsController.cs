using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TestNotificationBackend.Models;
using TestNotificationBackend.Services;

namespace TestNotificationBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public string GetHomePage()
        {
            return "Welcome to the TestPushNotification backend!";
        }

        [HttpPut]
        [Route("installations")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        public async Task<ActionResult<string[]>> CreateOrUpdateInstallation(
        [Required] DeviceInstallation deviceInstallation)
        {
            // Simulate token authentication
            //var result = new UserManagerService().GetUser(User.Identity.Name);
            var result = new UserManagerService().GetUserById(Guid.Parse(Request.Headers["User-Id"]));

            if (result == null)
                return Unauthorized();
            else
            {
                // Need regex to erase all spaces on tags, because are not allowed
                var success = await _notificationService
                .CreateOrUpdateInstallationAsync(deviceInstallation, HttpContext.RequestAborted, new string[] { Regex.Replace(result.Id.ToString(), " ", "") });

                if (success == null)
                    return UnprocessableEntity();

                return Ok(success);
            }
        }

        [HttpDelete()]
        [Route("installations/{installationId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        public async Task<IActionResult> DeleteInstallation(
            [Required][FromRoute] string installationId)
        {
            // Simulate token authentication
            if (new UserManagerService().GetUserById(Guid.Parse(Request.Headers["User-Id"])) == null)
                return new UnauthorizedResult();
            else
            {
                var success = await _notificationService
                .DeleteInstallationByIdAsync(installationId, CancellationToken.None);

                if (!success)
                    return new UnprocessableEntityResult();

                return new OkResult();
            }
        }

        [HttpPost]
        [Route("requests")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        public async Task<IActionResult> RequestPush(
            [Required] NotificationRequest notificationRequest)
        {
            if ((!notificationRequest.Silent &&
            string.IsNullOrWhiteSpace(notificationRequest?.Text)))
                return new BadRequestResult();

            var success = await _notificationService
                .RequestNotificationAsync(notificationRequest, HttpContext.RequestAborted);

            // This endpoint will return always 422 until APN configuration is not done. Otherwise, it will return 200.
            if (!success)
                return new UnprocessableEntityResult();

            return new OkResult();

        }
    }
}



