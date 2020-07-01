using Android.Widget;
using System;
using TestNotification.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestNotification
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthorizedUserPage : ContentPage
    {
        readonly INotificationRegistrationService _notificationRegistrationService;

        //Disable back button to avoid pop navigation
        protected override bool OnBackButtonPressed() => true;

        public AuthorizedUserPage(string username, string company, string sectorCompany)
        {
            InitializeComponent();

            _notificationRegistrationService = ServiceContainer.Resolve<INotificationRegistrationService>();

            usernameLabel.Text = "Username: <strong>" + username + "</strong>";
            companyLabel.Text = "Company: <strong>" + company + "</strong>";
            sectorCompanyLabel.Text = "Sector company: <strong>" + sectorCompany + "</strong>";
        }


        async void OnInfoAuthorizedUserClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoAuthorizedUserPage());
        }

        async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
            Toast.MakeText(Android.App.Application.Context, "Successful logout: device no longer registered.", ToastLength.Short).Show();

            usernameLabel.Text = "Username:";
            companyLabel.Text = "Company:";
            sectorCompanyLabel.Text = "Sector company:";

            DeregistrationDevice();
        }
        
        async void DeregistrationDevice()
        {
            await _notificationRegistrationService.DeregisterDeviceAsync().ContinueWith((task)
                => {
                    if (task.IsFaulted)
                        Console.WriteLine($"Exception: {task.Exception.Message}");
                    else
                    {
                        Console.WriteLine("Device deregistered: now is not longer available to receive push notification until next user login.");
                    }
                });
        }
    }
}