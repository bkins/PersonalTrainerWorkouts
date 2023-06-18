using System;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    public class ResolvedWorkoutsToExercise : ViewModelBase
    {
        public int                       Id                           { get; set; }
        public Workout                   Workout                      { get; set; }
        public Exercise                  Exercise                     { get; set; }
        public string                    LengthOfTime                 { get; set; }
        public int                       Reps                         { get; set; }
        public int                       OrderBy                      { get; set; }
        public LinkedWorkoutsToExercises TheLinkedWorkoutsToExercises { get; set; }

        public ResolvedWorkoutsToExercise(int workoutsToExerciseId)
        {
            try
            {
                Id                           = workoutsToExerciseId;
                TheLinkedWorkoutsToExercises = DataAccessLayer.GetLinkedWorkoutsToExercise(workoutsToExerciseId);

                Workout      = DataAccessLayer.GetWorkout(TheLinkedWorkoutsToExercises.WorkoutId);
                Exercise     = DataAccessLayer.GetExercise(TheLinkedWorkoutsToExercises.ExerciseId);
                LengthOfTime = TheLinkedWorkoutsToExercises.LengthOfTime;
                Reps         = TheLinkedWorkoutsToExercises.Reps;
                OrderBy      = TheLinkedWorkoutsToExercises.OrderBy;
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }
    }
}
