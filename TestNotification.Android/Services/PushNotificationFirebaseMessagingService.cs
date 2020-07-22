using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Firebase.Messaging;
using System;
using TestNotification.Services;

namespace TestNotification.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class PushNotificationFirebaseMessagingService : FirebaseMessagingService
    {
        INotificationRegistrationService _notificationRegistrationService;
        IDeviceInstallationService _deviceInstallationService;

        INotificationRegistrationService NotificationRegistrationService
            => _notificationRegistrationService ??
                (_notificationRegistrationService =
                ServiceContainer.Resolve<INotificationRegistrationService>());

        IDeviceInstallationService DeviceInstallationService
            => _deviceInstallationService ??
                (_deviceInstallationService =
                ServiceContainer.Resolve<IDeviceInstallationService>());

        public override void OnNewToken(string token)
        {
            DeviceInstallationService.Token = token;

            NotificationRegistrationService.RefreshRegistrationAsync()
                .ContinueWith((task) => { if (task.IsFaulted) throw task.Exception; });
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);

            // convert the incoming message to a local notification
            if (message.GetNotification() != null)
            {
                SendLocalNotification(message.GetNotification().Body);
                //MainActivity.badgeCount++;
            }
            else
                throw new Exception("Error during retrieving notification");
        }

        public void SendLocalNotification(string body)
        {
            // Set up an intent so that tapping the notifications returns to this app
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra("text", body);

            // Create a PendingIntent
            PendingIntent pendingIntent =
                PendingIntent.GetActivity(this, new Random().Next(), intent, PendingIntentFlags.OneShot);

            // Instantiate the builder and set notification elements
            var builder = new NotificationCompat.Builder(this, Constants.CHANNEL_ID)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetContentTitle("TestNotification")
                .SetContentText(body)
                .SetStyle(new NotificationCompat.BigTextStyle())
                .SetSmallIcon(Resource.Mipmap.launcher_foreground);
                //.AddAction(Resource.Drawable.notification_icon, "OK", pendingIntent);
            
            // Set priority, ringtone and vibration for Android 7.1 (API level 25) and lower
            if (Build.VERSION.SdkInt <= BuildVersionCodes.NMr1)
            {
                builder.SetPriority(NotificationCompat.PriorityHigh)
                    .SetDefaults(NotificationCompat.DefaultAll);
            }

            // Block screen visibility is available only after Android 5.0 (API 21)
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                builder.SetVisibility((int)NotificationVisibility.Private)
                    .SetCategory(Notification.CategoryMessage);

            // Get the notification manager
            var notificationManager = NotificationManagerCompat.From(this);

            // Publish the notification
            var notification = builder.Build();
            notificationManager.Notify(Constants.NOTIFICATION_ID, notification);

            //StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }
    }
}