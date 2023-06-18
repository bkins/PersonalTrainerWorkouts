using System;
using PersonalTrainerWorkouts.ViewModels.Tab_About;
using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.Views.Tab_About
{
    //BENDO: [Before release of version 1.0] Remove xamarin stuff and put in info about this app
    public partial class AboutPage
    {
        public AboutViewModel AboutViewModel { get; set; }
        public AboutPage()
        {
            InitializeComponent();

            AboutViewModel = new AboutViewModel();

            TablesLabel.Text = string.Join($"{Environment.NewLine}{Environment.NewLine}"
                                         , AboutViewModel.TableList);
        }

        async void OnButtonClicked(object sender
                                 , EventArgs e)
        {
            // Launch the specified URL in the system browser.
            await Launcher.OpenAsync("https://aka.ms/xamarin-quickstart");
        }

        private void
        TableHeaderLabel_OnTapped(object sender
                                , EventArgs e)
        {
            TableLabelScrollView.IsVisible = !TableLabelScrollView.IsVisible;
        }
    }
}
