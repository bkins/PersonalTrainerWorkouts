using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalTrainerWorkouts.ViewModels
{
    class SyncDbViewModel : ViewModelBase
    {
        public string DevicesDbPath  { get; set; }
        public string ExternalDbPath { get; set; }

        public SyncDbViewModel()
        {
            //DevicesDbPath = DataAccessLayer.
        }
    }
}
