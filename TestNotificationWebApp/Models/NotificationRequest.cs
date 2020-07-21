using System.ComponentModel.DataAnnotations;

namespace TestNotificationWebApp.Models
{
    public class NotificationRequest
    {
        [Required]
        [Display(Name = "Text notification: ")]
        public string Text { get; set; }

        // Tags's vector length = 10, because are allowed at maximum 10 tags
        //[Display(Name = "Tags notification: ")]
        //public string[] Tags { get; set; } = new string[10];
        [Display(Name = "Tag notification: ")]
        public string Tag { get; set; }
    }
}
