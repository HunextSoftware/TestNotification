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
        }

        static public uint countCheckbox = 0;
        static public bool isRegistered = false;

        //Checkbox events
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
        
        //Button events
        void OnSaveButtonClicked(object sender, EventArgs e)
        {
            /* 1) Simulate that this app has been installed on own device.
             * 2) Register the device on the push notification server, reserved for TestNotification.
             * --> if the method is async, use await
             */
            Console.WriteLine($"INFO: It has been clicked 'Registra device' button.");
            if (countCheckbox == 1)
            {
                if (!isRegistered)
                {
                    isRegistered = true;
                    //FIXME fix the constructor, in a way to insert correctly the parameters
                    _ = new AppDataToSend(1, 1);
                    Console.WriteLine($"OK: This device has been registered on the server.");
                }
                else Console.WriteLine($"ERROR: This device has registered on the server yet.");

            }
            else Console.WriteLine($"ERROR: Please, select only one voice.");

        }

        void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            /* 1) Simulate that this app has been removed by own device.
             * 2) Unregister the device by the push notification server, reserved for TestNotification
             * --> if the method is async, use await
             */
            Console.WriteLine($"INFO: It has been clicked 'Dissocia device' button.");
            if (isRegistered)
            {
                isRegistered = false;
                Console.WriteLine($"OK: This device has been unregistered by the server.");
            }
            else Console.WriteLine($"ERROR: You cannot unregister your device, cause is not registered to the server.");
        }
    }
}
