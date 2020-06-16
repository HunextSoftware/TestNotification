using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestNotification
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            //register the device on the push notification server
            //use await!!
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            //unregister the device by the push notification server
            //use await!!
        }
    }
}
