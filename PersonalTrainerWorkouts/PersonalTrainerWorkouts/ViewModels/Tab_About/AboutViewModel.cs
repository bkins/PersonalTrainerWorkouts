using System.Collections.Generic;

namespace PersonalTrainerWorkouts.ViewModels.Tab_About
{
    public class AboutViewModel : ViewModelBase
    {
        public List<string> TableList { get; set; }

        public AboutViewModel ()
        {
            TableList = DataAccessLayer.GetTables();
        }
    }
}