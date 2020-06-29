using TestNotification.Models;
using TestNotification.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestNotification
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            ServiceContainer.Resolve<ITestNotificationActionService>()
                .ActionTriggered += NotificationActionTriggered;

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        void NotificationActionTriggered(object sender, TestNotificationAction e)
            => ShowActionAlert(e);

        void ShowActionAlert(TestNotificationAction action)
            => MainThread.BeginInvokeOnMainThread(()
                => MainPage?.DisplayAlert("TestNotification", $"{action} action received", "OK")
                    .ContinueWith((task) => { if (task.IsFaulted) throw task.Exception; }));
    }
}
