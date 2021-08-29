namespace PersonalTrainerWorkouts.ViewModels
{
    internal class ConfigurationViewModel : ViewModelBase
    {
        public void CreateTables()
        {
            DataAccessLayer.CreateTables();
        }

        public void DropTables()
        {
            DataAccessLayer.DropTables();
        }
    }
}
