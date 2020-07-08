
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
            
            //response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginResponse>(responseBody);
            //return JObject.Parse(responseBody);
        }



        //return await SendAsync(HttpMethod.Post, RequestUrl, new LoginRequest(url, username, password)); 

        //private async Task SendAsync<T>(HttpMethod requestType, string requestUri, T obj)
        //{
        //    string serializedContent = null;

        //    await Task.Run(() => serializedContent = JsonConvert.SerializeObject(obj))
        //        .ConfigureAwait(false);

        //    await SendAsync(requestType, requestUri, serializedContent);
        //}

        ////private async Task SendAsync(HttpMethod requestType,
        //private async Task<HttpContent> SendAsync(HttpMethod requestType,
        //    string requestUri,
        //    string jsonRequest = null)
        //{
        //    var request = new HttpRequestMessage(requestType, new Uri($"{_baseApiUrl}{requestUri}"));

        //    if (jsonRequest != null)
        //        request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        //    var response = await _client.SendAsync(request).ConfigureAwait(false);

        //    response.EnsureSuccessStatusCode();
        //    return response.Content;
        //}
    }
}