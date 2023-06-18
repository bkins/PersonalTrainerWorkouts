using Avails.Xamarin;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Settings
{
    public class BackupDatabaseViewModel : ViewModelBase
    {
        public string DatabaseLocation => DataAccessLayer.DatabaseLocation;
        public string DatabaseName     => DataAccessLayer.DatabaseFileName;
        public string BackupLocation   { get; set; }
        
        private BackupRestoreDatabase BackupRestorer { get; set; }

        public BackupDatabaseViewModel(string destinationPath)
        {
            BackupLocation = destinationPath;

            BackupRestorer = new BackupRestoreDatabase(DatabaseLocation
                                                     , BackupLocation);
        }

        public string Backup()
        {
            var backedUpFileNameAndPath = BackupRestorer.Backup(DatabaseName);

            return backedUpFileNameAndPath;
        }
        
        public void Restore(string fileToRestoreFileName)
        {
            BackupRestorer.Restore(fileToRestoreFileName);
        }
    }
}
