using System.Collections.Generic;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public List<string> TableList { get; set; }

        public AboutViewModel ()
        {
            TableList = DataAccessLayer.GetTables();
        }
    }
}