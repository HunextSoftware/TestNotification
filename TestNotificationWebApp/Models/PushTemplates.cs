namespace TestNotificationWebApp.Models
{
    // TO DELETE!!
    public class PushTemplates
    {
        public class Generic
        {
            public const string Android = "{ \"notification\": { \"title\" : \"TestNotification\", \"body\" : \"$(alertMessage)\"} }";
            public const string iOS = "{ \"aps\" : {\"alert\" : \"$(alertMessage)\"} }";
        }

        // Never used in this prototype: silent notifications are a type of notification which have no priority (no sound and not visible at once) and visible only if the user pulls the shade down
        public class Silent
        {
            public const string Android = "{ \"data\" : {\"message\" : \"$(alertMessage)\"} }";
            public const string iOS = "{ \"aps\" : {\"content-available\" : 1, \"apns-priority\": 5, \"sound\" : \"\", \"badge\" : 0}, \"message\" : \"$(alertMessage)\" }";
        }
    }
}
