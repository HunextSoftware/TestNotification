using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestNotification.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestNotification
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthorizedUserPage : ContentPage
    {
        readonly INotificationRegistrationService _notificationRegistrationService;

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
            //deregistrazione dispositivo
            await Navigation.PushAsync(new MainPage());
            Toast.MakeText(Android.App.Application.Context, "Logout riuscito: dispositivo non più registrato.", ToastLength.Short).Show();
            usernameLabel.Text = "Nome utente:";
            companyLabel.Text = "Azienda:";
            sectorCompanyLabel.Text = "Settore aziendale:";
        }

        //TO DELETE
        void RegisterButtonClicked(object sender, EventArgs e)
            => _notificationRegistrationService.RegisterDeviceAsync().ContinueWith((task)
                => {
                    ShowAlert(task.IsFaulted ?
                       task.Exception.Message :
                       $"Device registered");
                });

        //TO DELETE
        void DeregisterButtonClicked(object sender, EventArgs e)
            => _notificationRegistrationService.DeregisterDeviceAsync().ContinueWith((task)
                => {
                    ShowAlert(task.IsFaulted ?
                      task.Exception.Message :
                      $"Device deregistered");
                });

        void ShowAlert(string message)
            => MainThread.BeginInvokeOnMainThread(()
                => Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show());

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {

        }
    }
}