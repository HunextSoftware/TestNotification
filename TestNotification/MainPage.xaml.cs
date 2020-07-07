using Android.Widget;
using LiteDB;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
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
        LoginService _loginService;

        // Disable back button to avoid pop navigation
        protected override bool OnBackButtonPressed() => true;

        public MainPage()
        {
            InitializeComponent();

            _notificationRegistrationService = ServiceContainer.Resolve<INotificationRegistrationService>();
            //TODO Is URL right??
            _loginService = new LoginService(Config.BackendServiceEndpoint);
        }


        async void OnInfoLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoLoginPage());
        }

        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if(urlEntry.Text != null && usernameEntry.Text != null && passwordEntry.Text != null)
            {
                await _loginService.Login(urlEntry.Text, usernameEntry.Text, passwordEntry.Text).ContinueWith(async (task)
                    => {
                        if (task.IsFaulted)
                            Toast.MakeText(Android.App.Application.Context, "Login error: inserted fields not right.", ToastLength.Long).Show();
                        else
                        {
                            //**************************TO RESTRUCTURE*************************************

                            // using (var db = new LiteDatabase("data.db"))
                            using (var db = new LiteDatabase((Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "data.db"))))
                            {
                                var collection = db.GetCollection<UserData>("UserData");
                                var result = collection.FindOne(x => x.Url.Equals(urlEntry.Text) && x.Username.Equals(usernameEntry.Text) && x.Password.Equals(passwordEntry.Text));

                            //*****************************************************************************
                                try
                                {
                                    await Navigation.PushAsync(new AuthorizedUserPage(result.Username, result.Company, result.SectorCompany));
                                    Toast.MakeText(Android.App.Application.Context, "Successful login: device registered.", ToastLength.Short).Show();

                                    // This row gives the possibility to insert tags, in a way that every user can be discriminated by the notification hub --> in our case, we need a GUID associated with the personal device 
                                    string[] tags = new string[] { Regex.Replace(result.GUID, " ", "") };

                                    // This block needs to recover AuthorizedUserPage activity, when the app is closed but the user has logged in yet
                                    string[] userDataAuthorized = { result.Username, result.Company, result.SectorCompany };
                                    var serializedUserDataAuthorized = JsonConvert.SerializeObject(userDataAuthorized);
                                    await SecureStorage.SetAsync(App.CachedDataAuthorizedUserKey, serializedUserDataAuthorized);

                                    RegistrationDevice(tags);
                                }
                                catch
                                {
                                    Toast.MakeText(Android.App.Application.Context, "Login error: inserted fields not right.", ToastLength.Long).Show();
                                }
                            }
                        }
                });
            } else
                Toast.MakeText(Android.App.Application.Context, "Error: fill in all fields.", ToastLength.Long).Show();
        }

        async void RegistrationDevice(string[] tags)
        {
            await _notificationRegistrationService.RegisterDeviceAsync(tags).ContinueWith(async (task)
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
