using Android.Text;
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

        static public uint countCheckbox = 0;
        static public bool isRegistered = false;


        async void OnInfoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoPage());
        }


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
            /* 
             * 1) Simulate this app has been installed on own device.
             * 2) Sign in the device into the push notification server, reserved for TestNotification.
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
                    Console.WriteLine($"OK: This device has been signed in on the server.");
                    Toast.MakeText(Android.App.Application.Context, "Il device è stato registrato nel server.", ToastLength.Long).Show();
                    stateLabel.Text = $"Stato device: <strong>registrato</strong>.";
                    stateLabel.TextColor = Color.Green;
                }
                else
                {
                    Console.WriteLine($"ERROR: This device is signed in on the server yet.");
                    Toast.MakeText(Android.App.Application.Context, "Errore: device già registrato nel server.", ToastLength.Long).Show();
                }    
            }
            else
            {
                Console.WriteLine($"ERROR: Please, select only one voice.");
                Toast.MakeText(Android.App.Application.Context, "Selezionare una sola voce.", ToastLength.Long).Show();
            }
        }

        void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            /* 
             * 1) Simulate this app has been uninstalled by own device.
             * 2) Unregister the device by the push notification server, reserved for TestNotification.
             */
            Console.WriteLine($"INFO: It has been clicked 'Dissocia device' button.");
            if (isRegistered)
            {
                isRegistered = false;
                Console.WriteLine($"OK: This device has been unregistered by the server.");
                Toast.MakeText(Android.App.Application.Context, "Il device è stato dissociato dal server.", ToastLength.Long).Show();
                stateLabel.Text = $"Stato device: <strong>non registrato</strong>.";
                stateLabel.TextColor = Color.Red;
            }
            else 
            {
                Console.WriteLine($"ERROR: You cannot delete your device's reference, cause is not signed in to the server.");
                Toast.MakeText(Android.App.Application.Context, "Errore: device non ancora registrato nel server.", ToastLength.Long).Show();
            } 
        }
    }
}
