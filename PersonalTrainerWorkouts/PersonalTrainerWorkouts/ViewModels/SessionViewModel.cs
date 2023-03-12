using System.Linq;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class SessionViewModel : BaseViewModel
    {
        private int                 Id                                 { get; set; }
        public  Session             NewSession                         { get; set; }
        public  ClientListViewModel ClientListViewModel                { get; set; }
        public  bool                ManageExerciseToolBarItemIsEnabled { get; set; }

        public SessionViewModel()
        {
            NewSession          = new Session();
            ClientListViewModel = new ClientListViewModel();
        }

        public SessionViewModel(string sessionId)
        {
            NewSession = DataAccessLayer.GetSessions()
                                        .FirstOrDefault(session => session.Id
                                                                          .ToString() == sessionId);

            ClientListViewModel = new ClientListViewModel();
        }

        public void SaveSession(string clientName)
        {
            NewSession.Client = DataAccessLayer.GetClients()
                                               .FirstOrDefault(client => client.DisplayName == clientName);
            
            if (NewSession.Id == 0)
            {
                Id = DataAccessLayer.AddNewSession(NewSession);
            }
            else
            {
                DataAccessLayer.UpdateSession(NewSession);
            }
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