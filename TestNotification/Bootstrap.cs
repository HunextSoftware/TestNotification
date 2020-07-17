using System;
using TestNotification.Services;

namespace TestNotification
{
    public static class Bootstrap
    {
        //The Begin method will be called by each platform when the app launches passing in a platform-specific implementation of IDeviceInstallationService.
        public static void Begin(Func<IDeviceInstallationService> deviceInstallationService)
        {
            ServiceContainer.Register(deviceInstallationService);

            ServiceContainer.Register<INotificationRegistrationService>(()
                => new NotificationRegistrationService(
                    Config.BackendServiceEndpoint));
        }
    }
}
