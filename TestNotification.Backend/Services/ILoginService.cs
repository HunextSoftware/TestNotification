using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TestNotificationBackend.Models;

namespace TestNotificationBackend.Services
{
    public interface ILoginService
    {
        Task<ActionResult<LoginResponse>> TryToLogin(LoginRequest loginRequest, CancellationToken token);
    }
}
