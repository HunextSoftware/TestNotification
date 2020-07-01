using Android.Widget;
using LiteDB;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using TestNotification.Models;
using TestNotification.Services;
using Xamarin.Forms;

namespace TestNotification
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        readonly INotificationRegistrationService _notificationRegistrationService;

        //Disable back button to avoid pop navigation
        protected override bool OnBackButtonPressed() => true;

        public MainPage()
        {
            InitializeComponent();

            _notificationRegistrationService = ServiceContainer.Resolve<INotificationRegistrationService>();
        }


        async void OnInfoLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoLoginPage());
        }

        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if(urlEntry.Text != null && usernameEntry.Text != null && passwordEntry.Text != null)
            {
                using (var db = new LiteDatabase((Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "data.db"))))
                {
                    var collection = db.GetCollection<UserData>("UserData");
                    var result = collection.FindOne(x => x.Url.Equals(urlEntry.Text) && x.Username.Equals(usernameEntry.Text) && x.Password.Equals(passwordEntry.Text));

                    try
                    {         
                        await Navigation.PushAsync(new AuthorizedUserPage(result.Username, result.Company, result.SectorCompany));
                        Toast.MakeText(Android.App.Application.Context, "Successful login: device registered.", ToastLength.Short).Show();

                        //TODO --> Is GUID needed as tag?? Understand it talking with tutor, explaining the reason why I chose not to put it
                        //Adding tags which correspond to company and sectorCompany
                        string[] tags = new string[] {Regex.Replace(result.Company, " ", ""), Regex.Replace(result.SectorCompany, " ", "")};

                        RegistrationDevice(tags);
                    }
                    catch
                    {
                        Toast.MakeText(Android.App.Application.Context, "Login error: inserted fields not right.", ToastLength.Long).Show();
                    }
                }
            } else
                Toast.MakeText(Android.App.Application.Context, "Error: fill in all fields.", ToastLength.Long).Show();
        }

        async void RegistrationDevice(string[] tags)
        {
            await _notificationRegistrationService.RegisterDeviceAsync(tags).ContinueWith((task)
                                    => {
                                        if (task.IsFaulted)
                                            Console.WriteLine($"Exception: {task.Exception.Message}");
                                        else
                                        {
                                            Console.WriteLine("Device registered: now is available to receive push notification.");
                                        }          
                                    });
        }
    }
}
