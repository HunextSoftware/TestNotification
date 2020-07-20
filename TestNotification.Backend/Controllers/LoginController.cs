using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestNotificationBackend.Models;
using TestNotificationBackend.Services;

namespace TestNotificationBackend.Controllers
{
    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<LoginResponse>> PostLogin([Required] LoginRequest login)
        {
            var usm = new UserManagerService();

            if (!usm.AuthenticateUser(login.Username, login.Password))
                return Unauthorized(new LoginResponse());
            else
            {
                var user = usm.GetUserByUsername(login.Username);
                return Ok(new LoginResponse(user.Username, user.Company, user.SectorCompany, user.Id));
            }    
        }
    }
}
