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

        //Tags are the same at every registration
        [Required]
        public string[] Tags { get; set; } = {/*"single:GUID",*/ "topic:Company" /*,"topic:SectorCompany"*/};
    }
}
