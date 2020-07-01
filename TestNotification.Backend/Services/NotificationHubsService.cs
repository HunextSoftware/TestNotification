using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TestNotificationBackend.Models;


namespace TestNotificationBackend.Services
{
    public class NotificationHubService : INotificationService
    {
        readonly NotificationHubClient _hub;
        readonly Dictionary<string, NotificationPlatform> _installationPlatform;
        readonly ILogger<NotificationHubService> _logger;

        public NotificationHubService(IOptions<NotificationHubOptions> options, ILogger<NotificationHubService> logger)
        {
            _logger = logger;
            _hub = NotificationHubClient.CreateClientFromConnectionString(
                options.Value.ConnectionString,
                options.Value.Name);

            _installationPlatform = new Dictionary<string, NotificationPlatform>
            {
                { nameof(NotificationPlatform.Apns).ToLower(), NotificationPlatform.Apns },
                { nameof(NotificationPlatform.Fcm).ToLower(), NotificationPlatform.Fcm }
            };
        }

        public async Task<bool> CreateOrUpdateInstallationAsync(DeviceInstallation deviceInstallation, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(deviceInstallation?.InstallationId) ||
                string.IsNullOrWhiteSpace(deviceInstallation?.Platform) ||
                string.IsNullOrWhiteSpace(deviceInstallation?.PushChannel) ||
                deviceInstallation.Tags.Equals(null))
                return false;

            var installation = new Installation()
            {
                InstallationId = deviceInstallation.InstallationId,
                PushChannel = deviceInstallation.PushChannel,
                Tags = deviceInstallation.Tags
            };

            if (_installationPlatform.TryGetValue(deviceInstallation.Platform, out var platform))
                installation.Platform = platform;
            else
                return false;

            try
            {
                await _hub.CreateOrUpdateInstallationAsync(installation, token);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteInstallationByIdAsync(string installationId, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(installationId))
                return false;

            try
            {
                await _hub.DeleteInstallationAsync(installationId, token);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> RequestNotificationAsync(NotificationRequest notificationRequest, CancellationToken token)
        {
            if ((notificationRequest.Silent &&
                string.IsNullOrWhiteSpace(notificationRequest?.Action)) ||
                (!notificationRequest.Silent &&
                (string.IsNullOrWhiteSpace(notificationRequest?.Text)) ||
                string.IsNullOrWhiteSpace(notificationRequest?.Action)))
                return false;

            var androidPushTemplate = notificationRequest.Silent ?
                PushTemplates.Silent.Android :
                PushTemplates.Generic.Android;

            var iOSPushTemplate = notificationRequest.Silent ?
                PushTemplates.Silent.iOS :
                PushTemplates.Generic.iOS;

            var androidPayload = PrepareNotificationPayload(
                androidPushTemplate,
                notificationRequest.Text,
                notificationRequest.Action);

            var iOSPayload = PrepareNotificationPayload(
                iOSPushTemplate,
                notificationRequest.Text,
                notificationRequest.Action);

            try
            {
                if (notificationRequest.Tags.Length == 0)
                {
                    // This will broadcast to all users registered in the notification hub
                    await SendPlatformNotificationsAsync(androidPayload, iOSPayload, token);
                }

                //else if (notificationRequest.Tags.Length <= 20)
                //{
                //    await SendPlatformNotificationsAsync(androidPayload, iOSPayload, notificationRequest.Tags, token);
                //}
                //else
                //{
                //    var notificationTasks = notificationRequest.Tags
                //        .Select((value, index) => (value, index))
                //        .GroupBy(g => g.index / 20, i => i.value)
                //        .Select(tags => SendPlatformNotificationsAsync(androidPayload, iOSPayload, tags, token));

                //    await Task.WhenAll(notificationTasks);
                //}

                // 6 is the tag limit for any boolean expression constructed using the logical operators AND (&&), OR (||), NOT (!)
                else if (notificationRequest.Tags.Length <= 6)
                {
                    var tagExpression = new System.Text.StringBuilder();
                    for (int i = 0; i < notificationRequest.Tags.Length; i++)
                    {
                        if (i == 0)
                            tagExpression.Append(notificationRequest.Tags[i] + " ");
                        else
                            tagExpression.Append("AND " + notificationRequest.Tags[i] + " ");
                    }

                    await SendPlatformNotificationsAsync(androidPayload, iOSPayload, tagExpression.ToString(), token);
                }
                else
                    _logger.LogError("Error: tags number not allowed. The valid tags number is from 0 to 6.");

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error sending notification");
                return false;
            }
        }

        string PrepareNotificationPayload(string template, string text, string action) => template
            .Replace("$(alertMessage)", text, StringComparison.InvariantCulture)
            .Replace("$(alertAction)", action, StringComparison.InvariantCulture);

        Task SendPlatformNotificationsAsync(string androidPayload, string iOSPayload, CancellationToken token)
        {
            var sendTasks = new Task[]
            {
                _hub.SendFcmNativeNotificationAsync(androidPayload, token),
                _hub.SendAppleNativeNotificationAsync(iOSPayload, token)
            };

            return Task.WhenAll(sendTasks);
        }

        //Task SendPlatformNotificationsAsync(string androidPayload, string iOSPayload, IEnumerable<string> tags, CancellationToken token)
        //{
        //    var sendTasks = new Task[]
        //    {
        //        _hub.SendFcmNativeNotificationAsync(androidPayload, tags, token),
        //        _hub.SendAppleNativeNotificationAsync(iOSPayload, tags, token)
        //    };

        //    return Task.WhenAll(sendTasks);
        //}

        Task SendPlatformNotificationsAsync(string androidPayload, string iOSPayload, string tagExpression, CancellationToken token)
        {
            var sendTasks = new Task[]
            {
                _hub.SendFcmNativeNotificationAsync(androidPayload, tagExpression, token),
                _hub.SendAppleNativeNotificationAsync(iOSPayload, tagExpression, token)
            };

            return Task.WhenAll(sendTasks);
        }

    }
}