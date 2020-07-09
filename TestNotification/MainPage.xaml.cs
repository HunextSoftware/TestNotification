using Android.Widget;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        readonly LoginService _loginService;

        // Disable back button to avoid pop navigation
        protected override bool OnBackButtonPressed() => true;

        public MainPage()
        {
            InitializeComponent();

            _notificationRegistrationService = ServiceContainer.Resolve<INotificationRegistrationService>();
            _loginService = new LoginService(Config.BackendServiceEndpoint);
        }


        async void OnInfoLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoLoginPage());
        }

        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if (urlEntry.Text != null && usernameEntry.Text != null && passwordEntry.Text != null)
            {
                try
                {
                    var result = await _loginService.Login(urlEntry.Text, usernameEntry.Text, passwordEntry.Text);

                    if (result.IsThereUser)
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
                    else
                        Toast.MakeText(Android.App.Application.Context, "Login error: inserted fields not right.", ToastLength.Long).Show();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    Toast.MakeText(Android.App.Application.Context, "Unexpected error: retry to log in.", ToastLength.Long).Show();
                }

            }
            else
                Toast.MakeText(Android.App.Application.Context, "Please, complete all the fields.", ToastLength.Long).Show();
        }

        async void RegistrationDevice(string[] tags)
        {
            await _notificationRegistrationService.RegisterDeviceAsync(tags).ContinueWith(async (task)
                                    =>
            {
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
