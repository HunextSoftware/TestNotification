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

            usernameLabel.Text = "Nome utente: <strong>" + username + "</strong>";
            companyLabel.Text = "Azienda: <strong>" + company + "</strong>";
            sectorCompanyLabel.Text = "Settore aziendale: <strong>" + sectorCompany + "</strong>";
        }


        async void OnInfoAuthorizedUserClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoAuthorizedUserPage());
        }

        async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
            Toast.MakeText(Android.App.Application.Context, "Logout riuscito: dispositivo non più registrato.", ToastLength.Short).Show();

            usernameLabel.Text = "Nome utente:";
            companyLabel.Text = "Azienda:";
            sectorCompanyLabel.Text = "Settore aziendale:";

            deregistrationDevice();
        }
        
        async void deregistrationDevice()
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