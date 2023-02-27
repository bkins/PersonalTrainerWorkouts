using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class SessionListViewModel : ViewModelBase
    {
        public ObservableCollection<Session>     ObservableListOfSessions { get; set; }
        public List<WorkoutsToExerciseViewModel> ListOfSessionsWithTotals { get; set; }
        
        public SessionListViewModel()
        {
            LoadData(DataAccessLayer.GetSessions());
        }

        public SessionListViewModel(List<Session> aListOfSessions, DataAccess dbAccessLayer)
        {
            DataAccessLayer          = dbAccessLayer;
            LoadData(aListOfSessions);
        }

        public void LoadData(IEnumerable<Session> sessionData)
        {
            
            ObservableListOfSessions = new ObservableCollection<Session>(sessionData);
        }
        
        public (string item, bool success) Delete(int index)
        {
            if (index > ObservableListOfSessions.Count - 1)
            {
                return (string.Empty, false);
            }

            //Get the workout to be deleted
            var itemToDelete = ObservableListOfSessions[index];
            var name         = itemToDelete.Name;

            //Remove the workout from the source list
            ObservableListOfSessions.RemoveAt(index);

            //Delete the Workout from the database
            var numberAffected = App.Database.DeleteSession(ref itemToDelete);

            ObservableListOfSessions = new ObservableCollection<Session>(DataAccessLayer.GetSessions());

            if (numberAffected == 0) {  return ("<Session was not deleted. See Logs>", false); }
            if (numberAffected > 1) { return ("<More than one Session was deleted!", true); }
            
            return (name, true);
        }
        
        public ObservableCollection<Session> SearchByClientName(string filterText)
        {
            return new ObservableCollection<Session>
                   (
                        ObservableListOfSessions.Where(field=>field.Client
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