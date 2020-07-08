using Newtonsoft.Json;

namespace TestNotification.Models
{
    public class LoginRequest
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("username")]
        public string Username{ get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public LoginRequest(string url, string username, string password)
        {
            Url = url;
            Username = username;
            Password = password;
        }

    }
}