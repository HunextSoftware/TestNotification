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
        const string CachedGUIDKey = "cached_guid";
        const string RequestUrl = "/api/notifications/installations";

        readonly string _baseApiUrl;
        readonly HttpClient _client;
        //IDeviceInstallationService _deviceInstallationService;

        public NotificationRegistrationService(string baseApiUri)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Accept", "application/json");

            _baseApiUrl = baseApiUri;
        }

        //IDeviceInstallationService DeviceInstallationService
        //    => _deviceInstallationService ??
        //        (_deviceInstallationService = ServiceContainer.Resolve<IDeviceInstallationService>());

        public async Task RegisterDeviceAsync(string guid)
        {
            var serializedGUID = JsonConvert.SerializeObject(guid);
            await SecureStorage.SetAsync(CachedGUIDKey, serializedGUID);

            await SendAsync(HttpMethod.Put, RequestUrl, guid)
                .ConfigureAwait(false);
        }

        public async Task RefreshRegistrationAsync()
        {
            var serializedGUID = await SecureStorage.GetAsync(CachedGUIDKey)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(serializedGUID))
                return;

            var guid = JsonConvert.DeserializeObject<string>(serializedGUID);

            await RegisterDeviceAsync(guid);
        }

        public async Task DeregisterDeviceAsync()
        {
            var serializedGUID = await SecureStorage.GetAsync(CachedGUIDKey)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(serializedGUID))
                throw new Exception("Unable to resolve an ID for the device.");

            SecureStorage.Remove(CachedGUIDKey);

            await SendAsync(HttpMethod.Delete, $"{RequestUrl}/{serializedGUID}");
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
    }
}
