using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Sessions
{
    public class SessionListViewModel : ViewModelBase
    {
        public ObservableCollection<Session> ObservableListOfSessions { get; set; }
        public List<WorkoutsToExerciseViewModel> ListOfSessionsWithTotals { get; set; }

        public SessionListViewModel()
        {
            var allSessions = DataAccessLayer.GetSessions();
            LoadData(allSessions);
        }

        public SessionListViewModel(List<Session> aListOfSessions, DataAccess dbAccessLayer)
        {
            DataAccessLayer = dbAccessLayer;
            LoadData(aListOfSessions);
        }

        public void LoadData(IEnumerable<Session> sessions)
        {

            ObservableListOfSessions = new ObservableCollection<Session>(sessions);
        }

        public (string item, bool success) Delete(int index)
        {
            if (index > ObservableListOfSessions.Count - 1)
            {
                return (string.Empty, false);
            }

            //Get the workout to be deleted
            var itemToDelete = ObservableListOfSessions[index];
            var name = itemToDelete.Name;

            //Remove the workout from the source list
            ObservableListOfSessions.RemoveAt(index);

            //Delete the Workout from the database
            var numberAffected = App.Database.DeleteSession(ref itemToDelete);

            ObservableListOfSessions = new ObservableCollection<Session>(DataAccessLayer.GetSessions());

            if (numberAffected == 0) { return ("<Session was not deleted. See Logs>", false); }
            if (numberAffected > 1) { return ("<More than one Session was deleted!", true); }

            return (name, true);
        }

        public ObservableCollection<Session> SearchByClientName(string filterText)
        {
            return new ObservableCollection<Session>
                   (
                        ObservableListOfSessions.Where(field => field.Client
                                                                   .DisplayName
                                                                   .ToUpper()
                                                                   .Contains(filterText.ToUpper()))
                   );
        }

        public ObservableCollection<Session> SearchSessions(string filterText)
        {

            return SearchByClientName(filterText);
        }
    }
}