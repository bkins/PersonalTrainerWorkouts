using System.Collections.Generic;
using System.Collections.ObjectModel;
using PersonalTrainerWorkouts.Models.Intermediates;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    public class WorkoutsToExerciseRawViewModel : ViewModelBase
    {
        public List<LinkedWorkoutsToExercises>                 WorkoutsToExercisesData { get; }
        public ObservableCollection<LinkedWorkoutsToExercises> ListOfWorkoutExercises  { get; set; }

        public WorkoutsToExerciseRawViewModel()
        {
            WorkoutsToExercisesData = new List<LinkedWorkoutsToExercises>(DataAccessLayer.GetAllLinkedWorkoutsToExercises());
            ListOfWorkoutExercises  = new ObservableCollection<LinkedWorkoutsToExercises>(WorkoutsToExercisesData);
        }
    }
}
