
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigurationPage : ContentPage
    {
        public ConfigurationPage()
        {
            InitializeComponent();
        }

        private void DropTablesButtonClicked(object    sender,
                                             EventArgs e)
        {
            App.Database.DropTables();
        }

        private void CreateTablesButtonClicked(object    sender,
                                               EventArgs e)
        {
            App.Database.CreateTables();
        }
    }
}