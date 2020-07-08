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
        readonly HttpClient _client;

        public LoginService(string baseApiUri)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Accept", "application/json");

            _baseApiUrl = baseApiUri;
        }

        public async Task<LoginResponse> Login(string url, string username, string password)
        {
            string serializedContent = null;
            await Task.Run(() => serializedContent = JsonConvert.SerializeObject(new LoginRequest(url, username, password))).ConfigureAwait(false);
            
            var httpContent = new StringContent(serializedContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(new Uri($"{_baseApiUrl}{RequestUrl}"), httpContent).ConfigureAwait(false);
            
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginResponse>(responseBody);
        }

        //public async Task<LoginResponse> Login(string url, string username, string password)
        //{
        //    string serializedContent = null;
        //    await Task.Run(() => serializedContent = JsonConvert.SerializeObject(new LoginRequest(url, username, password))).ConfigureAwait(false);

        //    var httpContent = new StringContent(serializedContent, Encoding.UTF8, "application/json");
        //    HttpResponseMessage response = await _client.PostAsync(new Uri($"{_baseApiUrl}{RequestUrl}"), httpContent).ConfigureAwait(false);

        //    response.EnsureSuccessStatusCode();
        //    string responseBody = await response.Content.ReadAsStringAsync();
        //    return JsonConvert.DeserializeObject<LoginResponse>(responseBody);
        //}
    }
}