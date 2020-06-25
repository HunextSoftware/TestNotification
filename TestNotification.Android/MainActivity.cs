﻿using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Util;
using Android.Gms.Common;
using System;
using Android.Content;
using Firebase.Iid;
using TestNotification.Droid.Services;
using TestNotification.Services;

namespace TestNotification.Droid
{
    [Activity(
        Label = "TestNotification",
        LaunchMode = LaunchMode.SingleTop, 
        Icon = "@mipmap/icon", 
        Theme = "@style/MainTheme", 
        MainLauncher = true, 
        ConfigurationChanges = 
        ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, Android.Gms.Tasks.IOnSuccessListener
    {
        ITestNotificationActionService _notificationActionService;
        IDeviceInstallationService _deviceInstallationService;

        ITestNotificationActionService NotificationActionService
            => _notificationActionService ??
                (_notificationActionService =
                ServiceContainer.Resolve<ITestNotificationActionService>());

        IDeviceInstallationService DeviceInstallationService
            => _deviceInstallationService ??
                (_deviceInstallationService =
                ServiceContainer.Resolve<IDeviceInstallationService>());

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Bootstrap.Begin(() => new DeviceInstallationService());

            if (DeviceInstallationService.NotificationsSupported)
            {
                FirebaseInstanceId.GetInstance(Firebase.FirebaseApp.Instance)
                    .GetInstanceId()
                    .AddOnSuccessListener(this);
            }

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App());

            ProcessNotificationActions(Intent);
        }






        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            ProcessNotificationActions(intent);
        }

        //Retrieve and store the Firebase token
        public void OnSuccess(Java.Lang.Object result)
            => DeviceInstallationService.Token =
                result.Class.GetMethod("getToken").Invoke(result).ToString();

        //Check whether a given Intent has an extra value named action
        void ProcessNotificationActions(Intent intent)
        {
            try
            {
                if (intent?.HasExtra("action") == true)
                {
                    var action = intent.GetStringExtra("action");

                    if (!string.IsNullOrEmpty(action))
                        NotificationActionService.TriggerAction(action);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        //public const string TAG = "MainActivity";
        //internal static readonly string CHANNEL_ID = "my_notification_channel";

        //protected override void OnCreate(Bundle savedInstanceState)
        //{
        //    TabLayoutResource = Resource.Layout.Tabbar;
        //    ToolbarResource = Resource.Layout.Toolbar;

        //    base.OnCreate(savedInstanceState);

        //    if (Intent.Extras != null)
        //    {
        //        foreach (var key in Intent.Extras.KeySet())
        //        {
        //            if (key != null)
        //            {
        //                var value = Intent.Extras.GetString(key);
        //                Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
        //            }
        //        }
        //    }

        //    IsPlayServicesAvailable();
        //    CreateNotificationChannel();

        //    Xamarin.Forms.Forms.SetFlags("RadioButton_Experimental");
        //    Xamarin.Essentials.Platform.Init(this, savedInstanceState);
        //    global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
        //    LoadApplication(new App());
        //}
        //public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        //{
        //    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        //    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        //}

        //// checks whether Google Play Services are available on the device
        //public bool IsPlayServicesAvailable()
        //{
        //    int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
        //    if (resultCode != ConnectionResult.Success)
        //    {
        //        if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
        //            Log.Debug(TAG, GoogleApiAvailability.Instance.GetErrorString(resultCode));
        //        else
        //        {
        //            Log.Debug(TAG, "This device is not supported");
        //            Finish();
        //        }
        //        return false;
        //    }

        //    Log.Debug(TAG, "Google Play Services is available.");
        //    return true;
        //}

        //// creates a notification channel
        //private void CreateNotificationChannel()
        //{
        //    if (Build.VERSION.SdkInt < BuildVersionCodes.O)
        //    {
        //        // Notification channels are new in API 26 (and not a part of the
        //        // support library). There is no need to create a notification
        //        // channel on older versions of Android.
        //        return;
        //    }

        //    var channelName = CHANNEL_ID;
        //    var channelDescription = string.Empty;
        //    var channel = new NotificationChannel(CHANNEL_ID, channelName, NotificationImportance.Default)
        //    {
        //        Description = channelDescription
        //    };

        //    var notificationManager = (NotificationManager)GetSystemService(NotificationService);
        //    notificationManager.CreateNotificationChannel(channel);
        //}
    }
}