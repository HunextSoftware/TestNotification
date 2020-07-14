﻿using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestNotification
{
    public partial class App : Application
    {
        public const string CachedDataAuthorizedUserKey = "cached_userdata_authorized";

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override async void OnStart()
        {
            try
            {
                var cachedData = await SecureStorage.GetAsync(CachedDataAuthorizedUserKey);
                var args = JsonConvert.DeserializeObject<string[]>(cachedData);

                MainPage = new NavigationPage(new AuthorizedUserPage(args[0], args[1], args[2]));
            }
            catch
            {
                MainPage = new NavigationPage(new MainPage());
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
