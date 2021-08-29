
using System;
using PersonalTrainerWorkouts.Utilities;
using PersonalTrainerWorkouts.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigurationPage : ContentPage
    {
        private ConfigurationViewModel ViewModel { get; }

        public ConfigurationPage()
        {
            InitializeComponent();
            ViewModel = new ConfigurationViewModel();
        }

        private void DropTablesButtonClicked(object    sender,
                                             EventArgs e)
        {
            ViewModel.DropTables();
        }

        private void CreateTablesButtonClicked(object    sender,
                                               EventArgs e)
        {
            ViewModel.CreateTables();
        }

        private async void ViewLogButtonClicked(object    sender,
                                                EventArgs e)
        {
            await PageNavigation.NavigateTo(nameof(MessageLog));
        }
    }
}