using Newtonsoft.Json;

namespace TestNotification.Models
{
    public class LoginResponse
    {
        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Company")]
        public string Company { get; set; }

        [JsonProperty("SectorCompany")]
        public string SectorCompany { get; set; }
    }
}