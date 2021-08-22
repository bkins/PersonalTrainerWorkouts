
using System;
using System.IO;
using PersonalTrainerWorkouts.Data;
using Xamarin.Forms;
namespace PersonalTrainerWorkouts
{
    public partial class App : Application
    {
        private static AsyncDatabase _asyncDatabase;
        private static Database      _database;
        private static DataStore     _dataStore;

        //public static DataStore DataStore => _dataStore ?? (_dataStore = new DataStore( AsyncDatabase ));
        private static readonly string Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WorkoutDatabase.db3");

        public static Database Database => _database = _database ?? new Database(Path);
       
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDg2MDY1QDMxMzkyZTMyMmUzME51QWlBT3A3WEdnc2E5WFd5YmtSSnBHMW5ZaENaUjV0VEt6a3haeUpYcWM9;NDg2MDY2QDMxMzkyZTMyMmUzMFkvOTkrOURkVGEyaERkbUVQbnJ5dUhGNzFKNWVwcUZDcTFiVHNoWDBtb289;NDg2MDY3QDMxMzkyZTMyMmUzMFZHUjNVVmRtc1VLVms1VG9iZjBQaW4wRXhiamUvUFB6OG0vTXZHK2NEVzQ9;NDg2MDY4QDMxMzkyZTMyMmUzMElUd1ZyZVVERk9WY24wd3BpdEI1WUFFc0w4V3lFdUxhbXBybUhibzdsVnM9;NDg2MDY5QDMxMzkyZTMyMmUzMFhFOXRxS1lRd0NjM3dVUDA1K1ZIVVNiZGFpcGlkM0pTSXViZWZkb0dNdXc9;NDg2MDcwQDMxMzkyZTMyMmUzMGRaZ2NJaCtDZzl3cG5BWFF3bTQ4enRFK3Z1emdacmJETWR6Z05CS25tc2c9;NDg2MDcxQDMxMzkyZTMyMmUzMGkxYjFJc2NiU01oQmRIclZuOFdvR0RFQVRROHNPK2lQcUtHZCtWOGU4MTQ9;NDg2MDcyQDMxMzkyZTMyMmUzMElKQVpNZm5yWXBBVmdodHlZbC8ybnE2SzJaY0lBTUdHL0d2MVR0R2NTbm89;NDg2MDczQDMxMzkyZTMyMmUzMFgwMHpMMlZVWGdBTnBRS1gxMW1xTi91MzlFUDlhQ293cnFpdU14bDFMZlU9;NDg2MDc0QDMxMzkyZTMyMmUzMFVPaGhVUWdPOWxCVVhucllyWEdUYktQODVYUzJZQkxpK0lrQ3dNN3UzS2c9");
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDYwMzg0QDMxMzkyZTMxMmUzME5yWGhoOENxR2NvTXNQenZZTWVnckRCZkwzV3R0SHh3bGJvWHZpc25Udjg9");
            InitializeComponent();
            
            MainPage = new AppShell();
            
            //BENDO: Look at site below. Some good ideas for handling/backing up database
            //https://causerexception.com/2019/06/28/sqlite-tips-and-tricks-for-mobile-developers/
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
