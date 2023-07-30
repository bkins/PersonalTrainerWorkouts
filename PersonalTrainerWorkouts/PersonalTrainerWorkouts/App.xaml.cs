using System;
using Android.OS;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Data;
using Environment = System.Environment;

namespace PersonalTrainerWorkouts
{
    public partial class App
    {
        //private static AsyncDatabase     _asyncDatabase;
        //private static DataStore         _dataStore;
        private static Database _database;
        private static ContactsDataStore _contactDataStore;

        private static readonly string Path = System.IO
                                                    .Path
                                                    .Combine(Environment.GetFolderPath(Environment.SpecialFolder
                                                                                                  .LocalApplicationData)
                                                           , "WorkoutDatabase.db3");

        public static Database Database => _database ??= new Database(Path);
        public static ContactsDataStore ContactDataStore => _contactDataStore ??= new ContactsDataStore();

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjUwNzkyNkAzMjMyMmUzMDJlMzBrU3NQZXhPZTF4ZTBDU21ITDh1dXJPYS81QjFodTBRZ3VBZjM4QnhVRlBrPQ==");
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt+QHFqVkNrXVNbdV5dVGpAd0N3RGlcdlR1fUUmHVdTRHRcQlliTH9QckVhWn9eeHM=;Mgo+DSMBPh8sVXJ1S0d+X1RPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSXpSc0RgXX9cc3RXQ2c=;ORg4AjUWIQA/Gnt2VFhhQlJBfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5Xd0BiWntecndVQmFb;MTczNTI0OUAzMjMxMmUzMTJlMzMzNWxvbWFGWm1lODJRRzlaNEpwYVlvVUF4RHpQZ0FxcWk1NG5hdVUwVWgwN0U9;MTczNTI1MEAzMjMxMmUzMTJlMzMzNW1PMUtQdkZ3TG5QZ2l6eW5tVE90UzBiVTN4bEcvcXVkNVNweDlWc0crS2c9;NRAiBiAaIQQuGjN/V0d+XU9Hc1RDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS31TckRmWHxacHZWR2dcVg==;MTczNTI1MkAzMjMxMmUzMTJlMzMzNVVRSzl1Q3dzWWxFL2ZLTlFITytiakl3ZVVEMlRzU1dMZklzcmplZmF3Njg9;MTczNTI1M0AzMjMxMmUzMTJlMzMzNVFIL1VLbVBSQTViSEV4dS9vRUR3b1ErbUtCN0tGYWwyeDBuYzVkUjlPa009;Mgo+DSMBMAY9C3t2VFhhQlJBfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5Xd0BiWntecndXQGJb;MTczNTI1NUAzMjMxMmUzMTJlMzMzNU0yTzkwZ1ZIa0JIallLTWJBL1Zsd25FN0I4KzhlY0VyV1NROFlSNXpBYkE9;MTczNTI1NkAzMjMxMmUzMTJlMzMzNVZEMjgwd3RZN0p0YnVBWXR3cmRUS2MzSnlDNmR0d1RFU1RabW1CWkFZbU09;MTczNTI1N0AzMjMxMmUzMTJlMzMzNVVRSzl1Q3dzWWxFL2ZLTlFITytiakl3ZVVEMlRzU1dMZklzcmplZmF3Njg9");

            InitializeComponent();

            MainPage = new AppShell();
            /*
             * StrictMode.setVmPolicy(new StrictMode.VmPolicy.Builder()
                .detectAll()
                .penaltyListener(Executors.newSingleThreadExecutor(), new StrictMode.OnVmViolationListener() {
                    @Override
                    public void onVmViolation(Violation v) {
                        //DO MY CUSTOM STUFF LIKE LOG IT TO CRASHLYTICS
                        Crashlytics.logException(v);
                    }
                })
                .penaltyLog()
                .build());
             */

            // StrictMode.SetVmPolicy(new StrictMode.VmPolicy.Builder()
            //                        .DetectLeakedClosableObjects()
            //                        .PenaltyLog()
            //                        .Build());
#if DEBUG
            StrictMode.EnableDefaults();
#endif



            //BENDO: [Before final release of v1.0] Look at site below. Some good ideas for handling/backing up database
            //https://danielcauserblog.wordpress.com/2019/06/28/sqlite-tips-and-tricks-for-mobile-developers/
        }

        protected override void OnStart()
        {
            AppDomain.CurrentDomain.UnhandledException += HandleGlobalException;
        }
        private void HandleGlobalException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            // Log or inspect the exception details here
            Logger.WriteLine(exception?.Message, Category.Error, exception);
        }

        protected override void OnSleep() { }

        protected override void OnResume() { }

    }
}
