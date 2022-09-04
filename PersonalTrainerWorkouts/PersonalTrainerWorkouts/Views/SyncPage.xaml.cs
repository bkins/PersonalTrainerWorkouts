using System;
using System.Windows.Input;
using PersonalTrainerWorkouts.Services;
using PersonalTrainerWorkouts.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SyncPage : ContentPage
    {
        public GoogleApiViewModel ViewModel { get; set; }
        
        public SyncPage()
        {
            InitializeComponent();
            ViewModel = new GoogleApiViewModel();
        }

        private void ExportButton_OnClicked(object    sender
                                          , EventArgs e)
        {
            ViewModel.TestDriveCommand.Execute(null);
        }

        private void TestGoogleDriveButton_OnClicked(object    sender
                                                   , EventArgs e)
        {
            var oauthRequest = "https://accounts.google.com/o/oauth2/v2/auth?response_type=code&scope=openid&" 
                             + $"redirect_uri={GoogleServices.RedirectUrl}&client_id={GoogleServices.ClientId}";
            
            Launcher.OpenAsync(new Uri(oauthRequest));

            ViewModel.LoginAuthenticator();
        }
    }
}
