using Android.Widget;
using LiteDB;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using TestNotification.LocalDatabase;
using TestNotification.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestNotification
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    { 
        public MainPage()
        {
            InitializeComponent();
        }

        async void OnInfoLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoLoginPage());
        }

        void OnEditorUrlChanged(object sender, TextChangedEventArgs e)
        {
            string oldText = e.OldTextValue;
            string newText = e.NewTextValue;
        }

        void OnEditorUsernameChanged(object sender, TextChangedEventArgs e)
        {
            string oldText = e.OldTextValue;
            string newText = e.NewTextValue;
        }

        void OnEditorPasswordChanged(object sender, TextChangedEventArgs e)
        {
            string oldText = e.OldTextValue;
            string newText = e.NewTextValue;
        }

        void OnEditorUrlCompleted(object sender, EventArgs e)
        {
            var text = ((Entry)sender).Text; 
        }

        void OnEditorUsernameCompleted(object sender, EventArgs e)
        {
            var text = ((Entry)sender).Text; 
        }

        void OnEditorPasswordCompleted(object sender, EventArgs e)
        {
            var text = ((Entry)sender).Text; 
        }

        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if(urlEntry.Text != null && usernameEntry.Text != null && passwordEntry.Text != null)
            {
                using (var db = new LiteDatabase((Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UserDataDb.db"))))
                {
                    var collection = db.GetCollection<UserData>("UserData");
                    var result = collection.Find(x => x.Url.Equals(urlEntry.Text) && x.Username.Equals(usernameEntry.Text) && x.Password.Equals(passwordEntry.Text));
                    if (result.Count() == 1)
                    {
                        //TODO Change "Navigation" -> it must be possible to turn back to Login Page only if user tap logout button
                        //FIXME solve this problem
                        await Navigation.PushAsync(new AuthorizedUserPage(result.Username, result.Company, result.SectorCompany));
                        Toast.MakeText(Android.App.Application.Context, "Login riuscito.", ToastLength.Short).Show();
                    } else
                        Toast.MakeText(Android.App.Application.Context, "Login non riuscito: campi inseriti non corretti.", ToastLength.Long).Show();
                }
            } else 
                Toast.MakeText(Android.App.Application.Context, "Errore: compilare tutti i campi.", ToastLength.Long).Show();
        }
    }
}
