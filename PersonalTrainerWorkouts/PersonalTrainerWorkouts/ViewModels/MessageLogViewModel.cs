using System;
using System.Collections.Generic;
using System.Text;
using PersonalTrainerWorkouts.Utilities;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class MessageLogViewModel : ViewModelBase
    {
        public string CompleteLog => Logger.CompleteLog;
    }
}
