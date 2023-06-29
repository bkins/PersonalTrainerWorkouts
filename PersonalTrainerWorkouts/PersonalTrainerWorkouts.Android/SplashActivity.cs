using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;

using Avails.D_Flat.Extensions;

using System.Collections.Generic;
using System.Threading.Tasks;
using Android;
using Android.Content.PM;
using AndroidX.Core.Content;
using AndroidX.Core.App;

namespace PersonalTrainerWorkouts.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash"
            , MainLauncher = true
            , NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        private const string Tag = "InternalLogging: " + nameof(PersonalTrainerWorkouts) + ":" + nameof(SplashActivity);

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
            Log.Debug(Tag
                    , "Performing pre-load work.");
            var preLoadTasks = new List<Task>
                               {
                                   GetPermissions()
                               };

            await Task.WhenAll(preLoadTasks);

            Log.Debug(Tag, "Pre-load work complete.");

            Log.Debug(Tag
                    , "Performing initial loading startup work.");
            var tasks = new List<Task>
                        {
                            LoadContactsFromDevice()
                          , InsertConfigurationValueIntoDatabase()
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
                //HACK: Busy wait for user to grant permissions
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
    }

}
