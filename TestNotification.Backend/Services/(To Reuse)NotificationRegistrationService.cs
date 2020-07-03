using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestNotification.Services;
using TestNotificationBackend.Models;
using Xamarin.Essentials;

namespace TestNotificationBackend.Services
{
    public class _To_Reuse_NotificationRegistrationServicee : INotificationRegistrationService
    {
        const string CachedTagsKey = "cached_tags";
        const string RequestUrl = "/api/notifications/installations";

        readonly string _baseApiUrl;
        readonly HttpClient _client;
        IDeviceInstallationService _deviceInstallationService;

        public NotificationRegistrationService(string baseApiUri)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Accept", "application/json");

            _baseApiUrl = baseApiUri;
        }

        IDeviceInstallationService DeviceInstallationService
            => _deviceInstallationService ??
                (_deviceInstallationService = ServiceContainer.Resolve<IDeviceInstallationService>());

        public Task DeregisterDeviceAsync()
        {
            var deviceId = DeviceInstallationService?.GetDeviceId();

            if (string.IsNullOrWhiteSpace(deviceId))
                throw new Exception("Unable to resolve an ID for the device.");

            SecureStorage.Remove(CachedTagsKey);

            return SendAsync(HttpMethod.Delete, $"{RequestUrl}/{deviceId}");
        }

        public async Task RegisterDeviceAsync(Guid guid)
        {
            await SendAsync(HttpMethod.Put, RequestUrl, guid)
                .ConfigureAwait(false);
        }

        public async Task RefreshRegistrationAsync()
        {
            var serializedTags = await SecureStorage.GetAsync(CachedTagsKey)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(serializedTags) ||
                string.IsNullOrWhiteSpace(DeviceInstallationService.Token))
                return;

            var tags = JsonConvert.DeserializeObject<string[]>(serializedTags);

            await RegisterDeviceAsync(tags);
        }

        private async Task SendAsync<T>(HttpMethod requestType, string requestUri, T obj)
        {
            string serializedContent = null;

            await Task.Run(() => serializedContent = JsonConvert.SerializeObject(obj))
                .ConfigureAwait(false);

            await SendAsync(requestType, requestUri, serializedContent);
        }

        private async Task SendAsync(HttpMethod requestType,
            string requestUri,
            string jsonRequest = null)
        {
            var request = new HttpRequestMessage(requestType, new Uri($"{_baseApiUrl}{requestUri}"));

            if (jsonRequest != null)
                request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        public Task RegisterDeviceAsync(params string[] tags)
        {
            throw new NotImplementedException();
        }
    }
}
