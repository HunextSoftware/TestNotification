using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestNotification.Models;
using Xamarin.Essentials;

namespace TestNotification.Services
{
    public class NotificationRegistrationService : INotificationRegistrationService
    {
        const string CachedDeviceTokenKey = "cached_device_token";
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


        public async Task RegisterDeviceAsync()
        {
            var deviceInstallation = DeviceInstallationService?.GetDeviceInstallation();

            if (deviceInstallation == null)
                throw new Exception($"Unable to get device installation information.");

            if (string.IsNullOrWhiteSpace(deviceInstallation.InstallationId))
                throw new Exception($"No {nameof(deviceInstallation.InstallationId)} value for {nameof(DeviceInstallation)}");

            if (string.IsNullOrWhiteSpace(deviceInstallation.Platform))
                throw new Exception($"No {nameof(deviceInstallation.Platform)} value for {nameof(DeviceInstallation)}");

            if (string.IsNullOrWhiteSpace(deviceInstallation.PushChannel))
                throw new Exception($"No {nameof(deviceInstallation.PushChannel)} value for {nameof(DeviceInstallation)}");

            // REFACTOR IT!!
            string serializedContent = null;
            await Task.Run(() => serializedContent = JsonConvert.SerializeObject(deviceInstallation)).ConfigureAwait(false);
            HttpResponseMessage response = await _client.PutAsync(new Uri($"{_baseApiUrl}{RequestUrl}"), new StringContent(serializedContent, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            await SecureStorage.SetAsync(CachedDeviceTokenKey, deviceInstallation.PushChannel)
                .ConfigureAwait(false);

            await SecureStorage.SetAsync(CachedTagsKey, JsonConvert.SerializeObject(JsonConvert.DeserializeObject<string[]>(await response.Content.ReadAsStringAsync())));
        }

        public async Task RefreshRegistrationAsync()
        {
            var cachedToken = await SecureStorage.GetAsync(CachedDeviceTokenKey)
                .ConfigureAwait(false);

            var serializedTags = await SecureStorage.GetAsync(CachedTagsKey)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(cachedToken) ||
                string.IsNullOrWhiteSpace(serializedTags) ||
                string.IsNullOrWhiteSpace(DeviceInstallationService.Token) ||
                cachedToken == DeviceInstallationService.Token)
                return;

            //var tags = JsonConvert.DeserializeObject<string[]>(serializedTags);

            await RegisterDeviceAsync();
        }

        public async Task DeregisterDeviceAsync()
        {
            var cachedToken = await SecureStorage.GetAsync(CachedDeviceTokenKey)
                .ConfigureAwait(false);

            if (cachedToken == null)
                return;

            var deviceId = DeviceInstallationService?.GetDeviceId();

            if (string.IsNullOrWhiteSpace(deviceId))
                throw new Exception("Unable to resolve an ID for the device.");

            await SendAsync(HttpMethod.Delete, $"{RequestUrl}/{deviceId}");
            
            SecureStorage.Remove(CachedDeviceTokenKey);
            SecureStorage.Remove(CachedTagsKey);
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

            await _client.SendAsync(request).ConfigureAwait(false);
        }
    }
}
