using System;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using Android.Util;

using Avails.D_Flat.Extensions;

using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.Content.PM;
using AndroidX.Core.Content;
using AndroidX.Core.App;
using Avails.GitHubService;
using Avails.Xamarin;
using AlertDialog = Android.App.AlertDialog;

namespace PersonalTrainerWorkouts.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash"
            , MainLauncher = true
            , NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        private const string Tag = "InternalLogging: " + nameof(PersonalTrainerWorkouts) + ":" + nameof(SplashActivity);

        private readonly ManualResetEvent _alertDismissedEvent = new ManualResetEvent(false);

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            if (!CheckPermissionGranted(Manifest.Permission.ReadExternalStorage)
             || !CheckPermissionGranted(Manifest.Permission.WriteExternalStorage)
             || !CheckPermissionGranted(Manifest.Permission.ReadContacts))
            {
                RequestPermission();
            }

            base.OnCreate(savedInstanceState, persistentState);
            Log.Debug(Tag, "SplashActivity.OnCreate");
        }

        /// <summary>
        /// Launches the startup task
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();

            var startupWork = new Task(Startup);
            startupWork.Start();
        }

        public override void OnBackPressed()
        {
            // Do nothing to prevent the back button from canceling the startup process
        }

        public override void OnRequestPermissionsResult(int          requestCode
                                                      , string[]     permissions
                                                      , Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode
                                                                 , permissions
                                                                 , grantResults);

            base.OnRequestPermissionsResult(requestCode
                                          , permissions
                                          , grantResults);
        }

        /// <summary>
        /// Simulates background work that happens behind the splash screen
        /// </summary>
        private async void Startup()
        {
            Log.Debug(Tag, "Performing pre-load work.");

            var preLoadTasks = new List<Task>
                               {
                                   GetPermissions()
                               };

            await Task.WhenAll(preLoadTasks);

            Log.Debug(Tag, "Pre-load work complete.");
            Log.Debug(Tag, "Performing initial loading startup work.");

            var tasks = new List<Task>
                        {
                            LoadContactsFromDevice()
                          , InsertConfigurationValueIntoDatabase()
                          , PopulateReleaseInfo()
                          , DisplayAlertIfTokenIsExpiring(this)
                            //, Add loading of resources that can be retrieve at start up here
                        };

            await Task.WhenAll(tasks);

            Log.Debug(Tag, "Initial loading startup work is complete. Starting MainActivity...");

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        private async Task PerformingInitialLoadingWork()
        {
            Log.Debug(Tag
                    , "Performing initial loading startup work.");
            var tasks = new List<Task>
                        {
                            LoadContactsFromDevice()
                          , InsertConfigurationValueIntoDatabase()
                            //, Add loading of resources that can be retrieve at start up here
                        };

            await Task.WhenAll(tasks);

        }

        private async Task PerformPreLoadWork()
        {
            Log.Debug(Tag
                    , "Performing pre-load work.");
            var preLoadTasks = new List<Task>
                               {
                                   GetPermissions()
                               };

            await Task.WhenAll(preLoadTasks);

            Log.Debug(Tag, "Pre-load work complete.");
        }

        private bool DoesNotHavePermissions()
        {
            return ! CheckPermissionGranted(Manifest.Permission.ReadExternalStorage)
                || ! CheckPermissionGranted(Manifest.Permission.WriteExternalStorage)
                || ! CheckPermissionGranted(Manifest.Permission.ReadContacts);
        }

        private Task GetPermissions()
        {
            if (DoesNotHavePermissions())
            {
                Task.Run(RequestPermission);
            }

            return Task.CompletedTask;
        }

        private bool CheckPermissionGranted(string permissions)
        {
            // Check if the permission is already available.
            return ContextCompat.CheckSelfPermission(this, permissions) == Permission.Granted;
        }

        private void RequestPermission()
        {
            ActivityCompat.RequestPermissions(this
                                            , new[]
                                              {
                                                  Manifest.Permission.ReadExternalStorage
                                                , Manifest.Permission.WriteExternalStorage
                                                , Manifest.Permission.ReadContacts
                                              }
                                            , 0);
        }

        private Task<bool> LoadContactsFromDevice()
        {
            while (DoesNotHavePermissions())
            {
                //HACK?: Busy wait for user to grant permissions
            }

            if (App.ContactDataStore.DeviceContacts.HasNoValue())
            {
                App.ContactDataStore.SetContacts();
            }
            return Task.FromResult(true);
        }

        private static Task<bool> InsertConfigurationValueIntoDatabase()
        {
            App.Database.InsertConfigurationValues();

            return Task.FromResult(true);
        }

        private static async Task PopulateReleaseInfo()
        {
            App.Releases = await GitHubService.GetReleases()
                                              .ConfigureAwait(false);
        }

        private Task DisplayAlertIfTokenIsExpiring(Activity activity)
        {
            var tokenIsAboutToExpire = TokenIsAboutToExpire();

            if ( ! tokenIsAboutToExpire.IsAboutToExpire) return Task.CompletedTask;

            activity.RunOnUiThread(() =>
            {
                var builder = new AlertDialog.Builder(activity);
                var message = new StringBuilder();
                message.AppendLine($"Your token will expire in {tokenIsAboutToExpire.DaysToExpiration} days.");
                message.AppendLine(string.Empty);
                message.Append("Go to Settings & Tools and apply a new token and expiration date.");

                builder.SetTitle("Token Expiring");
                builder.SetMessage(message.ToString());
                builder.SetPositiveButton("OK"
                                        , (sender
                                         , args) =>
                                          {
                                              _alertDismissedEvent.Set();
                                          });

                var alert = builder.Create();
                alert?.Show();
            });
            _alertDismissedEvent.WaitOne();

            return Task.CompletedTask;
        }

        private static (bool IsAboutToExpire, int DaysToExpiration) TokenIsAboutToExpire()
        {
            var expirationDate = Configuration.GitHubTokenExpirationDate;
            var daysUntilExpirationDate = CalculateDaysBetween(DateTime.Now
                                                             , expirationDate);

            return (daysUntilExpirationDate < Configuration.DaysToWarn, daysUntilExpirationDate);
        }
        private static int CalculateDaysBetween(DateTime startDate, DateTime endDate)
        {
            var timeSpan    = endDate - startDate;
            var daysBetween = timeSpan.Days;

            return daysBetween;
        }
    }

}
