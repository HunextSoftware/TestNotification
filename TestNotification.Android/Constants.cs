namespace TestNotification.Droid
{
    public static class Constants
    {
        // Azure's constants
        public const string ListenConnectionString = "Endpoint=sb://testpushnotificationhubns.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=JAqNrRS/eArWTIROSMjqYwzSlMWRuL9SB1E1Evyx3LQ=";
        public const string NotificationHubName = "testpushnotificationhub";

        // Notification channel's constants
        public const int NOTIFICATION_ID = 1000;
        public const string CHANNEL_ID = "TestNotificationChannel";
        public const string CHANNEL_NAME = "channel_test_notification";
        public const string CHANNEL_DESCRIPTION = "A notification channel for TestNotification app";

        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
    }
}