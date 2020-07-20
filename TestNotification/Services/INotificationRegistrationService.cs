using System.Threading.Tasks;

namespace TestNotification.Services
{
    //This will handle the interaction between the client and back-end service.
    public interface INotificationRegistrationService
    {
        Task DeregisterDeviceAsync();
        Task RegisterDeviceAsync();
        Task RefreshRegistrationAsync();
    }
}
