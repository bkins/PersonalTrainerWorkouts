using System;
using Avails.Xamarin;
using Avails.Xamarin.SecretSettings;
using PersonalTrainerWorkouts.Data;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts
{
    public partial class App : Application
    {
        private static AsyncDatabase          _asyncDatabase;
        private static Database               _database;
        private static DataStore              _dataStore;
        private static ContactsDataStore _contactDataStore;
        
        private static readonly string Path = System.IO
                                                    .Path
                                                    .Combine(Environment.GetFolderPath(Environment.SpecialFolder
                                                                                                  .LocalApplicationData)
                                                           , "WorkoutDatabase.db3");

        public static Database          Database         => _database = _database ?? new Database(Path);
        public static ContactsDataStore ContactDataStore => _contactDataStore = _contactDataStore ?? new ContactsDataStore();
        
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("ODM0NDE3QDMyMzAyZTM0MmUzMFBlamc3VjhPRS9mNXhTM2tPMXViZSt4VTR4T1Awb1RWT0x2alljd3FZakU9");
            
            InitializeComponent();

            MainPage = new AppShell();

            //BENDO: [Before final release of v1.0] Look at site below. Some good ideas for handling/backing up database
            //https://danielcauserblog.wordpress.com/2019/06/28/sqlite-tips-and-tricks-for-mobile-developers/
        }

        protected override void OnStart() { }

        protected override void OnSleep() { }

        protected override void OnResume() { }
    }
}
