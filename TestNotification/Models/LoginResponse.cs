using Newtonsoft.Json;

namespace TestNotification.Models
{
    public class LoginResponse
    {
        [JsonProperty("isThereUser")]
        public bool IsThereUser { get; set; }

        [JsonProperty("GUID")]
        public string GUID { get; set; }

        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Company")]
        public string Company { get; set; }

        [JsonProperty("SectorCompany")]
        public string SectorCompany { get; set; }
    }
}