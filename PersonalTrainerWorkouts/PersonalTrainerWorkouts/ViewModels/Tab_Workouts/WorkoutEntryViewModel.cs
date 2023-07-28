using Avails.ApplicationExceptions;
using Avails.D_Flat.Extensions;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    public class WorkoutEntryViewModel : ViewModelBase
    {
        private int     Id                                 { get; set; }
        public  Workout NewWorkout                         { get; set; }
        public  bool    ManageExerciseToolBarItemIsEnabled { get; set; }

        public WorkoutEntryViewModel()
        {
            NewWorkout = new Workout();
            Id         = 0;
        }

        public void SaveWorkout()
        {
            if (Id == 0)
            {
                Id = NewWorkout.Name.HasValue() 
                        ? DataAccessLayer.AddNewWorkout(NewWorkout) 
                        : throw new UnnamedEntityException($"{nameof(Workout)} was not named.  Must be named before attempting to save.");
            }
            else
            {
                DataAccessLayer.UpdateWorkout(NewWorkout);
            }
        }
    }
}
