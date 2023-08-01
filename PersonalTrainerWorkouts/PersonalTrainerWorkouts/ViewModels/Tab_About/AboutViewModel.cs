using System.Collections.Generic;
using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.ViewModels.Tab_About
{
    public class AboutViewModel : ViewModelBase
    {
        public List<string> TableList                     { get; set; }
        public string       CurrentVersion                { get; set; }
        public string       CurrentBuild                  { get; set; }
        public bool         IsTableLabelScrollViewVisible { get; set; }

        public AboutViewModel ()
        {
            TableList                     = DataAccessLayer.GetTables() ?? new List<string>();
            CurrentVersion                = VersionTracking.CurrentVersion;
            CurrentBuild                  = GetBuildName(VersionTracking.CurrentBuild);
            IsTableLabelScrollViewVisible = false;
        }

        private string GetBuildName(string buildNumber)
        {
            return buildNumber switch
                   {
                       "1" => "Alpha",
                       "2" => "Beta",
                       "3" => "RC",
                       "4" => "Prod",
                       _ => "Unknown"
                   };
        }
    }
}
