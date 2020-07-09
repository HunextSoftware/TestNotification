using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using TestNotificationBackend.Models;

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
            using (var db = new LiteDatabase("data.db"))
            {
                var collection = db.GetCollection<UserData>("UserData");
                var result = collection.FindOne(x => x.Url.Equals(login.Url) && x.Username.Equals(login.Username) && x.Password.Equals(login.Password));

                if (result == null)
                    return Unauthorized(new LoginResponse());
                else
                    return Ok(new LoginResponse(result.GUID, result.Username, result.Company, result.SectorCompany));
            }
        }
    }
}
