using System.Collections.Generic;
using Newtonsoft.Json;

namespace TestNotification.Models
{
    public class Login
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("username")]
        public string Username{ get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public Login(string url, string username, string password)
        {
            Url = url;
            Username = username;
            Password = password;
        }

    }
}