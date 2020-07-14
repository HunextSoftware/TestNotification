using Android.Widget;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
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
                loginActivityIndicator.IsRunning = true;
                try
                {
                    var result = await _loginService.Login(urlEntry.Text, usernameEntry.Text, passwordEntry.Text);
                    RegistrationDevice();

                    loginActivityIndicator.IsRunning = false;
                    await Navigation.PushAsync(new AuthorizedUserPage(result.Username, result.Company, result.SectorCompany));
                    Toast.MakeText(Android.App.Application.Context, "Successful login: device registered.", ToastLength.Short).Show();

                    // This block needs to recover AuthorizedUserPage activity, when the app is closed but the user has logged in yet
                    string[] userDataAuthorized = { result.Username, result.Company, result.SectorCompany };
                    await SecureStorage.SetAsync(App.CachedDataAuthorizedUserKey, JsonConvert.SerializeObject(userDataAuthorized));
                }
                catch (Exception ex)
                {
                    loginActivityIndicator.IsRunning = false;
                    Console.WriteLine($"Exception: {ex.Message}");
                    Toast.MakeText(Android.App.Application.Context, "Login error: inserted fields not right.", ToastLength.Long).Show();
                }
            }
            else
                Toast.MakeText(Android.App.Application.Context, "Please, complete all the fields.", ToastLength.Long).Show();
        }

        async void RegistrationDevice()
        {
            await _notificationRegistrationService.RegisterDeviceAsync().ContinueWith(async (task)
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
