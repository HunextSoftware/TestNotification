using Android.Widget;
using LiteDB;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using TestNotification.Models;
using TestNotification.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestNotification
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        readonly INotificationRegistrationService _notificationRegistrationService;
        const string CachedDataAuthorizedUserKey = "cached_userdata_authorized";

        //Disable back button to avoid pop navigation
        protected override bool OnBackButtonPressed() => true;

        public MainPage()
        {
            InitializeComponent();

            _notificationRegistrationService = ServiceContainer.Resolve<INotificationRegistrationService>();
        }


        async void OnInfoLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoLoginPage());
        }

        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if(urlEntry.Text != null && usernameEntry.Text != null && passwordEntry.Text != null)
            {
                // former-string
                using (var db = new LiteDatabase((Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "data.db"))))
                // Localhost backend
                //using (var db = new LiteDatabase("data.db"))
                // Azure backend (the same of localhost)
                //using (var db = new LiteDatabase(Config.BackendServiceEndpoint + "/dev/wwwroot/data.db"))
                {
                    var collection = db.GetCollection<UserData>("UserData");
                    var result = collection.FindOne(x => x.Url.Equals(urlEntry.Text) && x.Username.Equals(usernameEntry.Text) && x.Password.Equals(passwordEntry.Text));

                    try
                    {
                        await Navigation.PushAsync(new AuthorizedUserPage(result.Username, result.Company, result.SectorCompany));
                        Toast.MakeText(Android.App.Application.Context, "Successful login: device registered.", ToastLength.Short).Show();

                        // USEFUL TO RECOVER AuthorizedUserPage ACTIVITY, WHEN APP IS CLOSED AND THE USER IS LOGGED IN YET
                        string[] userDataAuthorized = { result.Username, result.Company, result.SectorCompany };
                        var serializeduserDataAuthorized = JsonConvert.SerializeObject(userDataAuthorized);
                        await SecureStorage.SetAsync(CachedDataAuthorizedUserKey, serializeduserDataAuthorized);

                        StartingRegistrationDevice(result.Id.ToString());
                    }
                    catch
                    {
                        Toast.MakeText(Android.App.Application.Context, "Login error: inserted fields not right.", ToastLength.Long).Show();
                    }
                }
            } else
                Toast.MakeText(Android.App.Application.Context, "Error: fill in all fields.", ToastLength.Long).Show();
        }

        async void StartingRegistrationDevice(string guid)
        {
            await _notificationRegistrationService.RegisterDeviceAsync(guid).ContinueWith(async (task)
                                    => {
                                        if (task.IsFaulted)
                                        {
                                            Console.WriteLine($"Exception: {task.Exception.Message}");
                                            await Navigation.PushAsync(new MainPage());
                                            Toast.MakeText(Android.App.Application.Context, "Error during device registration: retry to log in.", ToastLength.Long).Show();
                                        }
                                        else
                                            Console.WriteLine("Device registered: now is available to receive push notification."); 
                                    });
        }
    }
}
