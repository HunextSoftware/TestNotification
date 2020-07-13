using System.Threading;
using System.Threading.Tasks;
using TestNotificationBackend.Models;

namespace TestNotificationBackend.Services
{
    public interface INotificationService
    {
        Task<string[]> CreateOrUpdateInstallationAsync(DeviceInstallation deviceInstallation, CancellationToken requestAborted);
        Task<bool> DeleteInstallationByIdAsync(string installationId, CancellationToken token);
        Task<bool> RequestNotificationAsync(NotificationRequest notificationRequest, CancellationToken token);
    }
}