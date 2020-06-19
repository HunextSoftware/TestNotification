using Android.Widget;
using System;
using System.ComponentModel;
using TestNotification.Models;
using Xamarin.Forms;

namespace TestNotification
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    // For more details: https://docs.microsoft.com/it-it/xamarin/android/get-started/java-developers#event-handling
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        async void OnInfoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoPage());
        }

        static public uint countCheckbox = 0;
        static public bool isRegistered = false;

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

        void OnSaveButtonClicked(object sender, EventArgs e)
        {
            /* 1) Simulate that this app has been installed on own device.
             * 2) Register the device on the push notification server, reserved for TestNotification.
             */
            Console.WriteLine($"INFO: It has been clicked 'Registra device' button.");
            if (countCheckbox == 1)
            {
                if (!isRegistered)
                {
                    isRegistered = true;
                    /*
                     * Push notification server will receive this information probably by means of AppDataToSend object
                     * _ = new AppDataToSend(deviceId, orgSectorId); 
                    */
                    Console.WriteLine($"OK: This device has been registered on the server.");
                    Toast.MakeText(Android.App.Application.Context, "Your device is now signed in into the server.", ToastLength.Long).Show();
                    stateLabel.Text = $"Stato device: registrato.";
                    stateLabel.TextColor = Color.Green;
                }
                else
                {
                    Console.WriteLine($"ERROR: This device has registered on the server yet.");
                    Toast.MakeText(Android.App.Application.Context, "Error: you can't sign in your device twice.", ToastLength.Long).Show();
                }    
            }
            else
            {
                Console.WriteLine($"ERROR: Please, select only one voice.");
                Toast.MakeText(Android.App.Application.Context, "Please, select only one voice.", ToastLength.Long).Show();
            }
        }

        void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            /* 1) Simulate that this app has been removed by own device.
             * 2) Unregister the device by the push notification server, reserved for TestNotification
             */
            Console.WriteLine($"INFO: It has been clicked 'Dissocia device' button.");
            if (isRegistered)
            {
                isRegistered = false;
                Console.WriteLine($"OK: This device has been unregistered by the server.");
                Toast.MakeText(Android.App.Application.Context, "Your device's reference is now deleted by the server.", ToastLength.Long).Show();
                stateLabel.Text = $"Stato device: non registrato.";
                stateLabel.TextColor = Color.Red;
            }
            else 
            {
                Console.WriteLine($"ERROR: You cannot delete your device's reference, cause is not signed in to the server.");
                Toast.MakeText(Android.App.Application.Context, "Error: the device is not sign in into the server yet.", ToastLength.Short).Show();
            } 
        }
    }
}
