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

            var response = await SendAsync(HttpMethod.Put, $"{RequestUrl}", deviceInstallation);

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

            await RegisterDeviceAsync();
        }

        public async Task DeregisterDeviceAsync()
        {
            if (await SecureStorage.GetAsync(CachedDeviceTokenKey)
                .ConfigureAwait(false) == null)
                return;

            var deviceId = DeviceInstallationService?.GetDeviceId();

            if (string.IsNullOrWhiteSpace(deviceId))
                throw new Exception("Unable to resolve an ID for this device.");

            await SendAsync<object>(HttpMethod.Delete, $"{RequestUrl}/{deviceId}", null);

            // Remove User-Id key by the HTTP header: it needs to guarantee that this key can be inserted only in the installation phase and removed only in deinstallation phase
            if (_client.DefaultRequestHeaders.Contains("User-Id"))
                _client.DefaultRequestHeaders.Remove("User-Id");
            else
                Console.WriteLine("Warning: \"User-Id\" key is not part of HTTP header. It means that every user can enter into the system without authorization. Please solve this problem.");

            SecureStorage.Remove(CachedDeviceTokenKey);
            SecureStorage.Remove(CachedTagsKey);
        }

        private async Task<HttpResponseMessage> SendAsync<T>(HttpMethod requestType, string requestUri, T obj)
        {
            if (!_client.DefaultRequestHeaders.Contains("User-Id"))
            {
                // Add token authentication on header HTTP(S) request
                var tokenAuthentication = await SecureStorage.GetAsync(App.TokenAuthenticationKey);
                _client.DefaultRequestHeaders.Add("User-Id", tokenAuthentication);
            }

            var request = new HttpRequestMessage(requestType, new Uri($"{_baseApiUrl}{requestUri}"));

            if (obj != null)
            {
                string serializedContent = null;
                await Task.Run(() => serializedContent = JsonConvert.SerializeObject(obj)).ConfigureAwait(false);
                if (serializedContent != null)
                    request.Content = new StringContent(serializedContent, Encoding.UTF8, "application/json");
            }

            return await _client.SendAsync(request).ConfigureAwait(false);
        }
    }
}
