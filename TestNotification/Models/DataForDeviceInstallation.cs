using Newtonsoft.Json;
using System;

namespace TestNotification.Models
{
    // IS IT NEEDED??
    class DataForDeviceInstallation
    {
        [JsonProperty("installationId")]
        public Guid Guid { get; set; }
    }
}
