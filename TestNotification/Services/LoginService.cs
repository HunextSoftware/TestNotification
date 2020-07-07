
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestNotification.Models;

namespace TestNotification.Services
{
    public class LoginService
    {
        const string RequestUrl = "/login";

        readonly string _baseApiUrl;
        HttpClient _client;

        public LoginService(string baseApiUri)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Accept", "application/json");

            _baseApiUrl = baseApiUri;
        }

        // Develop this endpoint on the backend
        public async Task Login(string url, string username, string password)
        {
            await SendAsync<Login>(HttpMethod.Post, RequestUrl, new Login(url, username, password))
                .ConfigureAwait(false);
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