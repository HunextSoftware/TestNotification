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

        public AuthorizedUserPage(string username, string company, string sectorCompany)
        {
            InitializeComponent();

            _notificationRegistrationService = ServiceContainer.Resolve<INotificationRegistrationService>();

            usernameLabel.Text = "Nome utente: <strong style=\"color:blue\">" + username + "</strong>";
            companyLabel.Text = "Azienda: <strong style=\"color:blue\">" + company + "</strong>";
            sectorCompanyLabel.Text = "Nome utente: <strong style=\"color:blue\">" + sectorCompany + "</strong>";
        }

        async void OnInfoAuthorizedUserClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoAuthorizedUserPage());
        }




        // Set all the 3 labels:
        // Nome utente: xxxx
        // Azienda: yyyy
        // Settore: zzzz

        void RegisterButtonClicked(object sender, EventArgs e)
            => _notificationRegistrationService.RegisterDeviceAsync().ContinueWith((task)
                => {
                    ShowAlert(task.IsFaulted ?
                       task.Exception.Message :
                       $"Device registered");
                });

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