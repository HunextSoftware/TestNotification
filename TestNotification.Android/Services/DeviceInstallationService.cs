using System;
using Android.App;
using Android.Gms.Common;
using Java.Util;
using TestNotification.Models;
using TestNotification.Services;
using static Android.Provider.Settings;

namespace TestNotification.Droid.Services
{
    //This class provides a unique ID (using Secure.AndroidId) as part of the notification hub registration payload.
    public class DeviceInstallationService : IDeviceInstallationService
    {
        private string token;

        public string Token
        {
            get { return token; }
            set { 
                token = value; 
            }
        }


        public bool NotificationsSupported
            => GoogleApiAvailability.Instance
                .IsGooglePlayServicesAvailable(Application.Context) == ConnectionResult.Success;

        public string GetDeviceId()
            => Secure.GetString(Application.Context.ContentResolver, Secure.AndroidId);

        // a valid alternative
        //public string GetDeviceId()
        //   => UUID.RandomUUID().ToString();


        public DeviceInstallation GetDeviceInstallation(params string[] tags)
        {
            if (!NotificationsSupported)
                throw new Exception(GetPlayServicesError());

            var installation = new DeviceInstallation
            {
                InstallationId = GetDeviceId(),
                Platform = "fcm",
                PushChannel = Token
            };

            Console.WriteLine($"InstallationId: {installation.InstallationId}\n");
            Console.WriteLine($"Platform: {installation.Platform}\n");
            Console.WriteLine($"PushChannel: {installation.PushChannel}\n");

            installation.Tags.AddRange(tags);

            return installation;
        }

        string GetPlayServicesError()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Application.Context);

            if (resultCode != ConnectionResult.Success)
                return GoogleApiAvailability.Instance.IsUserResolvableError(resultCode) ?
                           GoogleApiAvailability.Instance.GetErrorString(resultCode) :
                           "This device is not supported";

            return "An error occurred preventing the use of push notifications";
        }
    }
}