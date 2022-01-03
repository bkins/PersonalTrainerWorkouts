using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PersonalTrainerWorkouts.Utilities;
using PersonalTrainerWorkouts.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigurationPage : ContentPage
    {
        private ConfigurationViewModel  ViewModel        { get; }
        private BackupDatabaseViewModel BackupViewModel  { get; set; }

        public  string                  BackupFolderPath => GetBackupDestinationPath();

        private static string GetBackupDestinationPath()
        {
            var backupFolder = Path.Combine("storage"
                                          , "emulated"
                                          , "0"
                                          , "Downloads"
                                          , "BackupDb");

            if ( ! Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }

            return backupFolder;
        }

        public ConfigurationPage()
        {
            InitializeComponent();
            ViewModel       = new ConfigurationViewModel();
            BackupViewModel = new BackupDatabaseViewModel(BackupFolderPath);
        }

        private void DropTablesButtonClicked(object    sender
                                           , EventArgs e)
        {
            ViewModel.DropTables();
        }

        private void CreateTablesButtonClicked(object    sender
                                             , EventArgs e)
        {
            ViewModel.CreateTables();
        }

        private async void ViewLogButtonClicked(object    sender
                                              , EventArgs e)
        {
            await PageNavigation.NavigateTo(nameof(MessageLog));
        }

        private void BackupDatabaseButton_OnClicked(object    sender
                                                  , EventArgs e)
        {
            BackupDatabase();
        }

        private async void BackupDatabase()
        {
            var backedUpFileNameAndPath = BackupViewModel.Backup();
            
            await DisplayAlert("DB Backed Up"
                             , GetBackedUpMessageText(backedUpFileNameAndPath)
                             , "OK");
        }
        
        private static string GetBackedUpMessageText(string backedUpFileNameAndPath)
        {
            var message = new StringBuilder();
            message.AppendLine("The database was backed up to:");
            message.AppendLine(backedUpFileNameAndPath);
            
            return message.ToString();
        }

        private void RestoreDatabaseButton_OnClicked(object    sender
                                                   , EventArgs e)
        {
            RestoreDatabase();
        }
        
        private async void RestoreDatabase()
        {
            var restoreOk = await DisplayAlert("Database Restore"
                                             , GetRestoredMessageText()
                                             , "OK"
                                             , "Canel");

            if (! restoreOk)
                return;

            var fileToRestore = await PickAndShow(PickOptions.Default);

            if ( ! await GetPermissions())
            {
                return;
            }

            try
            {
                BackupViewModel.Restore(fileToRestore);
            }
            catch (UnauthorizedAccessException  accessException)
            {
                await DisplayAlert("Error"
                                 , accessException.Message
                                 , "OK");
            }
        }
        
        async Task<string> PickAndShow(PickOptions options)
        {
            try
            {
                var result = await FilePicker.PickAsync(options);
                
                return result.FileName;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Cancelled or Error"
                                 , ex.Message
                                 , "OK");
            }
    
            return null;
        }

        private async Task<bool> GetPermissions()
        {
            var status = await CheckAndRequestPermissionAsync(new Permissions.StorageRead());
            if (status != PermissionStatus.Granted)
            {
                // Notify user permission was denied
                return false;
            }
            
            status = await CheckAndRequestPermissionAsync(new Permissions.StorageWrite());
            return status == PermissionStatus.Granted;
        }
        
        public async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>(T permission)
        where T : Permissions.BasePermission
        {
            var status = await permission.CheckStatusAsync();
            if (status != PermissionStatus.Granted)
            {
                status = await permission.RequestAsync();
            }

            return status;
        }

        private string GetRestoredMessageText()
        {
            var message = new StringBuilder();
            message.AppendLine($"The restore process will look in the folder {BackupFolderPath} for the database to restore from.");
            message.AppendLine("Select the version the database file to restore.");
            message.AppendLine("Tap Cancel to cnacel the restore process");

            return message.ToString();
        }

    }
}
