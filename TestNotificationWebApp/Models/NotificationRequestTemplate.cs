namespace TestNotificationWebApp.Models
{
    public class NotificationRequestTemplate
    {
        public const string body = "{ \"text\": \"$(textNotification)\", \"tags\": [ \"$(tagsNotification)\" ] }";
    }
}
