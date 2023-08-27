using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avails.GitHubService;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using Avails.Xamarin.Utilities;
using PersonalTrainerWorkouts.ViewModels;
using PersonalTrainerWorkouts.ViewModels.Tab_Settings;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views.Tab_SettingAndTools
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigurationPage
    {
        private ConfigurationViewModel  ViewModel        { get; }
        private BackupDatabaseViewModel BackupViewModel  { get; set; }

        public  string                  BackupFolderPath => GetBackupDestinationPath();

        public ConfigurationPage()
        {
            InitializeComponent();

            ViewModel       = new ConfigurationViewModel();
            BackupViewModel = new BackupDatabaseViewModel(BackupFolderPath);

            _tokenEditor.Text          = Configuration.GitHubToken;
            _expirationDatePicker.Date = Configuration.GitHubTokenExpirationDate == DateTime.MinValue
                                                            ? DateTime.Today.AddYears(1)
                                                            : Configuration.GitHubTokenExpirationDate;
            _applyButton.IsEnabled     = false;

            _toggleRefreshTokenControlsSwitch.IsToggled = false;
            ShowRefreshTokenControls();
        }

        private void ShowRefreshTokenControls()
        {
            _applyNewTokenLabel.IsVisible   = _toggleRefreshTokenControlsSwitch.IsToggled;
            _tokenEditor.IsVisible          = _toggleRefreshTokenControlsSwitch.IsToggled;
            _pasteButton.IsVisible          = _toggleRefreshTokenControlsSwitch.IsToggled;
            _expirationDatePicker.IsVisible = _toggleRefreshTokenControlsSwitch.IsToggled;
            _applyButton.IsVisible          = _toggleRefreshTokenControlsSwitch.IsToggled;
        }

        private static string GetBackupDestinationPath()
        {
            //var backupFolder = Path.Combine("storage"
            //                              , "emulated"
            //                              , "0"
            //                              , "Downloads"
            //                              , "BackupDb");
            string backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                                             , "BackupDb");
            
            if ( ! Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }

            return backupFolder;
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

        private void CreateContactTablesButton_OnClicked(object    sender
                                                       , EventArgs e)
        {
            ViewModel.CreateContactTables();
        }

        private void DropContactTablesButton_OnClicked(object    sender
                                                     , EventArgs e)
        {
            ViewModel.DropContactTables();
        }
        
        private void ViewLogButtonClicked(object    sender
                                              , EventArgs e)
        {
            //await PageNavigation.NavigateTo(nameof(MessageLog));
            
            var pageName = nameof(Avails.Xamarin.Views.LoggingPage.MessageLog);
            try
            {
                Logger.Verbose = true;
                PageNavigation.NavigateTo(pageName);
            }
            catch (Exception exception)
            {
                Logger.WriteLineToToastForced($"Could not open page {pageName}"
                                            , Category.Error
                                            , exception);
                Logger.Verbose = false;
            }
        }

        private void BackupDatabaseButton_OnClicked(object    sender
                                                  , EventArgs e)
        {
            BackupDatabase();
        }

        private async void BackupDatabase()
        {
            var backedUpFileNameAndPath = BackupViewModel.Backup();

            var emailFile = await DisplayAlert("DB Backed Up"
                                             , GetBackedUpMessageText(backedUpFileNameAndPath)
                                             , "Yes"
                                             , "No");

            if (emailFile)
            {
                //BENDO: Look Is & Os for example of sending files as attachments
            }
        }
        
        private static string GetBackedUpMessageText(string backedUpFileNameAndPath)
        {
            var message = new StringBuilder();
            message.AppendLine("The database was backed up to:");
            message.AppendLine(backedUpFileNameAndPath);
            message.AppendFormat(string.Empty);
            message.AppendLine("Would you like to email the backup file?");
            
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
            message.AppendLine("Tap Cancel to cancel the restore process");

            return message.ToString();
        }

        private async void PasteButtonOnClicked(object    sender
                                        , EventArgs e)
        {
            var token = await UiUtilities.GetClipboardValueAsync()
                                         .ConfigureAwait(false);

            _tokenEditor.Text = token;

            var tokenValid = await GitHubService.IsTokenValid(token);

            if (tokenValid)
            {
                Logger.WriteLineToToastForced("Token is valid!", Category.Information);
                Configuration.GitHubToken = token;
                _applyButton.IsEnabled    = true;
                return;
            }

            Logger.WriteLineToToastForced("Token is NOT valid!!!!", Category.Warning);
            _applyButton.IsEnabled = false;
        }

        private void ApplyButtonOnClicked(object    sender
                                        , EventArgs e)
        {
            Configuration.GitHubTokenExpirationDate = _expirationDatePicker.Date;
        }

        private void ToggleRefreshTokenControlsSwitchOnToggled(object           sender
                                                             , ToggledEventArgs e)
        {
            ShowRefreshTokenControls();
        }
    }
}
