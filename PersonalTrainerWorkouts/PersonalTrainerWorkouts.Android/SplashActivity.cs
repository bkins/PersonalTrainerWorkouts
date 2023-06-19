using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;

using Avails.D_Flat.Extensions;

using System.Collections.Generic;
using System.Threading.Tasks;

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

        /// <summary>
        /// Simulates background work that happens behind the splash screen
        /// </summary>
        private async void Startup()
        {
            Log.Debug(Tag, "Performing some startup work that takes a bit of time.");

            var tasks = new List<Task>
                        {
                            LoadContactsFromDevice()
                          , InsertConfigurationValueIntoDatabase()
                        //, Add loading of resources that can be retrieve at start up here
                        };

            await Task.WhenAll(tasks);

            Log.Debug(Tag, "Startup work is finished - starting MainActivity.");

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        private static Task<bool> LoadContactsFromDevice()
        {
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
