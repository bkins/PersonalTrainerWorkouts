using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.ComponentModel;
using System.Windows.Input;
using PersonalTrainerWorkouts.Services;
using PersonalTrainerWorkouts.ViewModels.HelperClasses;
using Xamarin.Auth;
using Xamarin.Auth.Presenters;
using Xamarin.Forms;


namespace PersonalTrainerWorkouts.ViewModels
{
    public class GoogleApiViewModel : INotifyPropertyChanged
    {
        private const string Scope        = "https://www.googleapis.com/auth/drive.file";
        private const string ClientId    = "365524202742-vvf78896mt59p7l64s70udj71rfghvmm.apps.googleusercontent.com";
        private const string RedirectUrl = "com.companyname.personaltrainerworkouts:/oauth2redirect";

        private OAuth2Authenticator              Authenticator { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand TestDriveCommand { get;  set; }

        public GoogleApiViewModel()
        {
            Authenticator = new OAuth2Authenticator
                                (
                                    GoogleServices.ClientId,
                                    string.Empty,
                                    Scope,
                                    new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
                                    new Uri(GoogleServices.RedirectUrl),
                                    new Uri("https://www.googleapis.com/oauth2/v4/token"),
                                    isUsingNativeUI: true
                                );

            AuthenticatorHelper.OAuth2Authenticator =  Authenticator;
            Authenticator.Completed                 += OnAuthOnCompleted;

            Authenticator.Error += (_, _) =>
                                   {

                                   };

            TestDriveCommand = new Command(() =>
                            {
                                var presenter = new OAuthLoginPresenter();
                                presenter.Login(Authenticator);
                            });
        }

        public void LoginAuthenticator()
        {
            var presenter = new OAuthLoginPresenter();
            presenter.Login(Authenticator);
        }

        private async void OnAuthOnCompleted(object                          sender
                                           , AuthenticatorCompletedEventArgs e)
        {
            if ( ! e.IsAuthenticated)
                return;

            var initializer = new GoogleAuthorizationCodeFlow.Initializer
                              {
                                  ClientSecrets = new ClientSecrets()
                                                  {
                                                      ClientId = ClientId
                                                     ,
                                                  }
                                 ,
                                  Scopes = new[]
                                           {
                                               Scope
                                           }
                                 ,
                                  DataStore = new FileDataStore("Google.Apis.Auth")
                              };

            var codeFlow = new GoogleAuthorizationCodeFlow(initializer);
            var userId   = "DriveTest";

            var token = new TokenResponse()
                        {
                            AccessToken      = e.Account.Properties["access_token"]
                          , ExpiresInSeconds = Convert.ToInt64(e.Account.Properties["expires_in"])
                          , RefreshToken     = e.Account.Properties["refresh_token"]
                          , Scope            = e.Account.Properties["scope"]
                          , TokenType        = e.Account.Properties["token_type"]
                        };

            var userCredential = new UserCredential(codeFlow
                                                  , userId
                                                  , token);

            var driveService = new DriveService(new BaseClientService.Initializer()
                                                {
                                                    HttpClientInitializer = userCredential
                                                  , ApplicationName       = "PersonalTrainerWorkouts"
                                                   ,
                                                });

            //test google drive
            var service = new GoogleDriveService(driveService);
            var fileId  = await service.CreateFile();

            await service.SaveFile(fileId
                                 , "testFileName"
                                 , "test content");

            var content = await service.ReadFile(fileId);
        }
    }

    public static class AuthenticatorHelper
    {
        public static OAuth2Authenticator OAuth2Authenticator { get; set; }
    }
    
}
