using System.Linq;
using Avails.Xamarin.Logger;
using NLog.Fluent;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels.Tab_Clients;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Sessions
{
    public class SessionViewModel : ViewModelBase
    {
        private int                 Id                                 { get; set; }
        public  Session             NewSession                         { get; set; }
        public  ClientListViewModel ClientListViewModel                { get; set; }
        public  bool                ManageExerciseToolBarItemIsEnabled { get; set; }

        public SessionViewModel()
        {
            NewSession = new Session();
            ClientListViewModel = new ClientListViewModel();
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

            }

            ClientListViewModel = new ClientListViewModel();
        }

        public void SaveSession(string clientName)
        {
            NewSession.Client = DataAccessLayer.GetClients()
                                               .FirstOrDefault(client => client.DisplayName == clientName);

            SaveSession();
        }

        public bool SaveSession()
        {
            if ( ! SessionIsValid()) return false;

            if (NewSession?.Id == 0)
            {
                NewSession.ClientId = NewSession.Client.Id;
                DataAccessLayer.AddNewSession(NewSession);
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

            if ( ! startBeforeEnd)
            {
                Logger.WriteLineToToastForced("Start date/time must be before the End date/time."
                                            , Category.Warning);

                return false;
            }

            return true;
        }
        public int Delete()
        {
            if (Id == 0)
            {
                return 0;
            }

            return DataAccessLayer.DeleteSession(NewSession);
        }
    }
}
