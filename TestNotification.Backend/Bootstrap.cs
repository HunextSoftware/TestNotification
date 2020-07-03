using System;
using TestNotificationBackend.Services;

namespace TestNotificationBackend
{
    public static class Bootstrap
    {
        //The Begin method will be called by each platform when the app launches passing in a platform-specific implementation of IDeviceInstallationService.
        public static void Begin(Func<IDeviceInstallationService> deviceInstallationService)
        {
            ServiceContainer.Register(deviceInstallationService);

            ServiceContainer.Register<ITestNotificationActionService>(()
                => new TestNotificationActionService());

            ServiceContainer.Register<INotificationRegistrationService>(()
                => new NotificationRegistrationService(
                    Config.BackendServiceEndpoint));
        }
    }
}
