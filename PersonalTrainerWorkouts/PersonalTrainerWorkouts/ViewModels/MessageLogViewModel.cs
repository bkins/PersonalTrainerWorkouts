using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avails.Xamarin.Logger;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class MessageLogViewModel
    {
        public string                        CompleteLog       => Logger.CompleteLog;
        public ObservableCollection<LogLine> LogAsList         => new ObservableCollection<LogLine>(Logger.ToList());
        public bool                          ShowSearchOptions { get; set; }
    }
}
