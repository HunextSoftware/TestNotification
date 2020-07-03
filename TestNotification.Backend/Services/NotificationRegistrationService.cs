﻿using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestNotification.Models;
using Xamarin.Essentials;

namespace TestNotificationBackend.Services
{
    public class NotificationRegistrationService : INotificationRegistrationService
    {
        const string CachedTagsKey = "cached_tags";
        // const string CachedDeviceTokenKey = "cached_device_token";
        const string RequestUrl = "/api/notifications/installations";

        readonly string _baseApiUrl;
        readonly HttpClient _client;
        IDeviceInstallationService _deviceInstallationService;

        //public NotificationRegistrationService(string baseApiUri, string apiKey)
        public NotificationRegistrationService(string baseApiUri)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            //_client.DefaultRequestHeaders.Add("apikey", apiKey);

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

        public async Task RegisterDeviceAsync(params string[] tags)
        {
            var deviceInstallation = DeviceInstallationService?.GetDeviceInstallation(tags);

            if (deviceInstallation == null)
                throw new Exception($"Unable to get device installation information.");

            if (string.IsNullOrWhiteSpace(deviceInstallation.InstallationId))
                throw new Exception($"No {nameof(deviceInstallation.InstallationId)} value for {nameof(DeviceInstallation)}");

            if (string.IsNullOrWhiteSpace(deviceInstallation.Platform))
                throw new Exception($"No {nameof(deviceInstallation.Platform)} value for {nameof(DeviceInstallation)}");

            if (string.IsNullOrWhiteSpace(deviceInstallation.PushChannel))
                throw new Exception($"No {nameof(deviceInstallation.PushChannel)} value for {nameof(DeviceInstallation)}");

            if (deviceInstallation.Tags.Equals(null))
                throw new Exception($"No {nameof(deviceInstallation.Tags)} value for {nameof(DeviceInstallation)}");

            await SendAsync<DeviceInstallation>(HttpMethod.Put, RequestUrl, deviceInstallation)
                .ConfigureAwait(false);

            var serializedTags = JsonConvert.SerializeObject(tags);
            Console.WriteLine($"SerializedTags: {serializedTags}");
            await SecureStorage.SetAsync(CachedTagsKey, serializedTags);
            Console.WriteLine($"Deserialize values of CachedTagsKey: {SecureStorage.GetAsync(CachedTagsKey)}");
        }

        public async Task RefreshRegistrationAsync()
        {
            // var cachedToken = await SecureStorage.GetAsync(CachedDeviceTokenKey).ConfigureAwait(false);

            var serializedTags = await SecureStorage.GetAsync(CachedTagsKey)
                .ConfigureAwait(false);

            /*if (string.IsNullOrWhiteSpace(cachedToken) ||
                string.IsNullOrWhiteSpace(serializedTags) ||
                string.IsNullOrWhiteSpace(DeviceInstallationService.Token) ||
                cachedToken == DeviceInstallationService.Token)
                return;*/

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
    }
}