using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels.Tab_Clients;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Sessions
{
    public class SessionViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private int                           Id                                 { get; set; }
        public  Session                       NewSession                         { get; set; }
        public  ClientListViewModel           ClientListViewModel                { get; set; }
        public  bool                          ManageExerciseToolBarItemIsEnabled { get; set; }

        private ObservableCollection<Workout> _workouts;
        public ObservableCollection<Workout> WorkoutsCollection
        {
            get => _workouts;
            set => _workouts = value;
        }


        private ObservableCollection<Workout> _selectedWorkouts;
        public ObservableCollection<Workout> SelectedWorkouts
        {
            get => _selectedWorkouts;
            set
            {
                _selectedWorkouts = value;
                OnPropertyChanged();
            }
        }
        public SessionViewModel()
        {
            NewSession           = new Session();
            //TODO:  these values are not being used. need to find out why
            NewSession.StartDate = DateTime.Now;
            NewSession.EndDate   = NewSession.StartDate.AddHours(1);

            ClientListViewModel  = new ClientListViewModel();

            WorkoutsCollection = new ObservableCollection<Workout>(DataAccessLayer.GetWorkouts());
            var sessionWorkouts = DataAccessLayer.GetSessions()
                                                 .SingleOrDefault(session => session.Id == NewSession.Id)
                                                 ?.Workouts;
            SelectedWorkouts = sessionWorkouts is null
                                        ? new ObservableCollection<Workout>()
                                        : new ObservableCollection<Workout>(sessionWorkouts);
        }

        public SessionViewModel(string sessionId)
        {
            if (sessionId == "0")
            {
                NewSession = new Session();
            }
            else
            {
                var allSessions = DataAccessLayer.GetSessions();

                NewSession = allSessions.FirstOrDefault(session => session.Id.ToString() == sessionId);

                WorkoutsCollection = new ObservableCollection<Workout>(DataAccessLayer.GetWorkouts());
                var sessionWorkouts = DataAccessLayer.GetSessions()
                                                     .SingleOrDefault(session => session.Id == NewSession.Id)
                                                     ?.Workouts;
                SelectedWorkouts = sessionWorkouts is null
                    ? new ObservableCollection<Workout>()
                    : new ObservableCollection<Workout>(sessionWorkouts);
            }

            ClientListViewModel = new ClientListViewModel();
        }

        public bool SaveSession()
        {
            if ( ! SessionIsValid()) return false;

            if (NewSession?.Id == 0)
            {
                NewSession.ClientId = NewSession.Client.Id;

                DataAccessLayer.AddSession(NewSession
                                         , NewSession.Client
                                         , NewSession.Workouts);
            }
            else
            {
                DataAccessLayer.UpdateSession(NewSession);
            }

            return true;
        }

        private bool SessionIsValid()
        {
            var hasClient      = NewSession?.Client is not null;
            var startBeforeEnd = NewSession?.StartDate < NewSession?.EndDate;

            if ( ! hasClient)
            {
                Logger.WriteLineToToastForced("Could not save without a selected client."
                                            , Category.Warning);

                return false;
            }

            if (startBeforeEnd) return true;

            Logger.WriteLineToToastForced("Start date/time must be before the End date/time."
                                        , Category.Warning);

            return false;

        }
        public int Delete()
        {
            return Id == 0 ? 0 : DataAccessLayer.DeleteSession(NewSession);

        }
    }
}
