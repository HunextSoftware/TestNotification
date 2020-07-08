using Newtonsoft.Json;
using TestNotification.Models;
using TestNotification.Services;
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

            ServiceContainer.Resolve<ITestNotificationActionService>()
                .ActionTriggered += NotificationActionTriggered;

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

        //TODO change this rows
        void NotificationActionTriggered(object sender, TestNotificationAction e)
            => ShowActionAlert(e);

        void ShowActionAlert(TestNotificationAction action)
            => MainThread.BeginInvokeOnMainThread(()
                => MainPage?.DisplayAlert("TestNotification", $"{action} action received", "OK")
                    .ContinueWith((task) => { if (task.IsFaulted) throw task.Exception; }));
    }
}
