using System.Collections.ObjectModel;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class WorkoutListViewModel : ViewModelBase
    {
        public ObservableCollection<Workout> ListOfWorkouts
        {
            get => new ObservableCollection<Workout>(DataAccessLayer.GetWorkouts());
            set { }
        }

        public WorkoutListViewModel()
        {
            ListOfWorkouts = new ObservableCollection<Workout>(DataAccessLayer.GetWorkouts());
        }

        public string Delete(int index)
        {
            if (index > ListOfWorkouts.Count - 1)
            {
                return string.Empty;
            }

            //Get the workout to be deleted
            var workoutToDelete = ListOfWorkouts[index];
            var workoutName     = workoutToDelete.Name;

            //Remove the workout from the source list
            ListOfWorkouts.RemoveAt(index);

            //Delete the Workout from the database
            App.Database.DeleteWorkout(ref workoutToDelete);

            ListOfWorkouts = new ObservableCollection<Workout>(DataAccessLayer.GetWorkouts());

            return workoutName;
        }
    }
}
