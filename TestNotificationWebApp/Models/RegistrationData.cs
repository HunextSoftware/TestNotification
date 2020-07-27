using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestNotificationWebApp.Models
{
    // Catch fundamental data from RegistrationDescription class of Microsoft.Azure.NotificationHubs
    public class RegistrationData
    {
        [Required]
        public string RegistrationId { get; set; }
        [Required]
        public string[] Tags { get; set; } = new string[10];
        [Required]
        public DateTime? ExpirationTime { get; set; }
    }
}
