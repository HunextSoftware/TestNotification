using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TestNotificationBackend.Models;

namespace TestNotificationBackend.Services
{
    public class LoginService
    {
        //readonly ILogger<LoginService> _logger;
        //readonly ILiteDatabase _liteDatabase;

        //public LoginService(ILogger<LoginService> logger, string connectionStringDB = "data.db")
        //{
        //    _logger = logger;
        //    _liteDatabase = new LiteDatabase(connectionStringDB);
        //}

        //public Task<ActionResult<LoginResponse>> TryToLogin(LoginRequest loginRequest, CancellationToken token)
        //{

        //    var collection = _liteDatabase.GetCollection<UserData>("UserData");
        //    var result = collection.FindOne(x => x.Url.Equals(loginRequest.Url) && x.Username.Equals(loginRequest.Username) && x.Password.Equals(loginRequest.Password));

        //    //return UnauthorizedResult(new LoginResponse(false));

        //}

    }
}
