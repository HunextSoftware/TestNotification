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
        ////readonly ILoginService _loginService;
        //readonly ILiteDatabase _liteDatabase;

        //public LoginController(/*ILoginService loginService,*/ string connectionStringDB = "data.db")
        //{
        //    //_loginService = loginService;
        //    _liteDatabase = new LiteDatabase(connectionStringDB);
        //}

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<LoginResponse>> PostLogin([Required] LoginRequest login)
        {
            using (var db = new LiteDatabase("data.db"))
            {
                var collection = db.GetCollection<UserData>("UserData");
                var result = collection.FindOne(x => x.Url.Equals(login.Url) && x.Username.Equals(login.Username) && x.Password.Equals(login.Password));

                if (result.Equals(null))
                    return Unauthorized(new LoginResponse(false));
                else
                    return Ok(new LoginResponse(true, result.GUID, result.Username, result.Company, result.SectorCompany));
            }



            //var success = await _loginService.TryToLogin(login, HttpContext.RequestAborted);

            //var task = new Task<ActionResult<LoginResponse>>( () =>
            //{
            //    using (_liteDatabase)
            //    {
            //        var collection = _liteDatabase.GetCollection<UserData>("UserData");
            //        var result = collection.FindOne(x => x.Url.Equals(login.Url) && x.Username.Equals(login.Username) && x.Password.Equals(login.Password));

            //        if (result.Equals(null))
            //            return Unauthorized(new LoginResponse(false));
            //        else
            //            return Ok(new LoginResponse(true, result.GUID, result.Username, result.Company, result.SectorCompany));
            //    }
            //});

            //return await task;

            
        }
    }
}
