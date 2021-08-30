using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Views
{
    //BENDO: [Before release of version 1.0] Remove xamarin stuff and put in info about this app
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        async void OnButtonClicked(object sender, EventArgs e)
        {
            // Launch the specified URL in the system browser.
            await Launcher.OpenAsync("https://aka.ms/xamarin-quickstart");
        }
    }
}