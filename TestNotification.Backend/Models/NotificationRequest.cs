using System;
using System.ComponentModel.DataAnnotations;

namespace TestNotificationBackend.Models
{
    public class NotificationRequest
    {
        [Required]
        public string Text { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>();

        // Silent = true when a notification is visible only if the user opens the drop-down menu
        public bool Silent { get; set; }
    }
}