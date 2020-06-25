using TestNotification.Models;

namespace TestNotification.Services
{
    //This interface will be implemented and bootstrapped by each target later to provide the platform-specific functionality and DeviceInstallation information required by the backend service.
    public interface IDeviceInstallationService
    {
        string Token { get; set; }
        bool NotificationsSupported { get; }
        string GetDeviceId();
        DeviceInstallation GetDeviceInstallation(params string[] tags);
    }
}
