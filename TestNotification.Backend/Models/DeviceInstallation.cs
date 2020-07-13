using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestNotificationBackend.Models
{
    public class DeviceInstallation
    {
        [Required]
        public string InstallationId { get; set; }

        [Required]
        public string Platform { get; set; }

        [Required]
        public string PushChannel { get; set; }
    }
}
