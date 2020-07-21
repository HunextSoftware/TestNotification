using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<string[]> CreateOrUpdateInstallationAsync(DeviceInstallation deviceInstallation, CancellationToken token, string[] tags)
        {
            if (string.IsNullOrWhiteSpace(deviceInstallation?.InstallationId) ||
                string.IsNullOrWhiteSpace(deviceInstallation?.Platform) ||
                string.IsNullOrWhiteSpace(deviceInstallation?.PushChannel))
                return null;

            var installation = new Installation()
            {
                InstallationId = deviceInstallation.InstallationId,
                PushChannel = deviceInstallation.PushChannel,
                Tags = tags
            };

            if (_installationPlatform.TryGetValue(deviceInstallation.Platform, out var platform))
                installation.Platform = platform;
            else
                return null;

            try
            {
                await _hub.CreateOrUpdateInstallationAsync(installation, token);
            }
            catch
            {
                return null;
            }

            return tags;
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
            if ((!notificationRequest.Silent &&
                    string.IsNullOrWhiteSpace(notificationRequest?.Text)))
                return false;

            var androidPushTemplate = notificationRequest.Silent ?
                PushTemplates.Silent.Android :
                PushTemplates.Generic.Android;

            var iOSPushTemplate = notificationRequest.Silent ?
                PushTemplates.Silent.iOS :
                PushTemplates.Generic.iOS;

            var androidPayload = PrepareNotificationPayload(
                androidPushTemplate,
                notificationRequest.Text);

            var iOSPayload = PrepareNotificationPayload(
                iOSPushTemplate,
                notificationRequest.Text);

            try
            {
                if (notificationRequest.Tags.Length == 0)
                {
                    // This will broadcast to all users registered in the notification hub
                    await SendPlatformNotificationsAsync(androidPayload, iOSPayload, token);
                }

                // 10 is the tag limit for any boolean expression constructed using the AND logical operator (&&) and no one OR (||)
                else if (notificationRequest.Tags.Length <= 10)
                {
                    // Build the expression about which tags are involved in the notification, bringing tags by JSON request body
                    var tagExpression = new System.Text.StringBuilder();
                    for (int i = 0; i < notificationRequest.Tags.Length; i++)
                    {
                        if (i == 0)
                            tagExpression.Append("(" + notificationRequest.Tags[i]);
                        else
                            tagExpression.Append(" && " + notificationRequest.Tags[i]);

                        if ((i + 1) == notificationRequest.Tags.Length)
                            tagExpression.Append(")");
                    }

                    await SendPlatformNotificationsAsync(androidPayload, iOSPayload, tagExpression.ToString(), token);
                }
                else
                {
                    _logger.LogError("Error: tags number not allowed. The valid tags number is from 0 to 10.");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error sending notification");
                return false;
            }
        }

        string PrepareNotificationPayload(string template, string text) => template
            .Replace("$(alertMessage)", text, StringComparison.InvariantCulture);

        Task SendPlatformNotificationsAsync(string androidPayload, string iOSPayload, CancellationToken token)
        {
            var sendTasks = new Task[]
            {
                _hub.SendFcmNativeNotificationAsync(androidPayload, token),
                _hub.SendAppleNativeNotificationAsync(iOSPayload, token)
            };

            return Task.WhenAll(sendTasks);
        }

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