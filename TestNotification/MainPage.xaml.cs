using System;
using System.ComponentModel;
using TestNotification.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestNotification
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    // For more details: https://docs.microsoft.com/it-it/xamarin/android/get-started/java-developers#event-handling
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        readonly INotificationRegistrationService _notificationRegistrationService;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            _notificationRegistrationService = ServiceContainer.Resolve<INotificationRegistrationService>();
        }

        static public uint countCheckbox = 0;
        static public bool isRegistered = false;

        async void OnInfoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoPage());
        }

        //FIXME: IS THIS BLOCK NEEDED??
        //**********************************************Checkbox events*************************************************

        void OnPayrollCheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value) countCheckbox++;
            else countCheckbox--;
            Console.WriteLine($"INFO: Payroll checkbox. Total number checkboxes on: {countCheckbox}.");
        }

        void OnConsultingCheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value) countCheckbox++;
            else countCheckbox--;
            Console.WriteLine($"INFO: Consulting checkbox. Total number checkboxes on: {countCheckbox}.");
        }

        void OnSoftwareCheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value) countCheckbox++;
            else countCheckbox--;
            Console.WriteLine($"INFO: Software checkbox. Total number checkboxes on: {countCheckbox}.");
        }


        //***********************************************Button events****************************************************

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
                => DisplayAlert("TestNotification", message, "OK").ContinueWith((task)
                    => { if (task.IsFaulted) throw task.Exception; }));

    }
}
