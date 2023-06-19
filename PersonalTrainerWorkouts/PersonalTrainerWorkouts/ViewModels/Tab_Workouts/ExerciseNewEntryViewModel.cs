using System;
using Avails.D_Flat.Exceptions;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    [Obsolete("Need to determine if I should simply remove this ViewModel or if it is not refeerenced in err")]
    public class ExerciseNewEntryViewModel : ViewModelBase
    {
        public Exercise NewExercise { get; set; }

        public ExerciseNewEntryViewModel()
        {
            NewExercise = new Exercise();
        }

        public void UpdateNewExercise()
        {
            DataAccessLayer.UpdateExercise(NewExercise);
        }

        public void SaveExercise(int workoutId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewExercise.Name))
                    throw new UnnamedEntityException($"{nameof(Exercise)} was not named.  Must be named before attempting to save.");

                var workout    = DataAccessLayer.GetWorkout(workoutId);
                var exerciseId = DataAccessLayer.AddNewExercise(NewExercise);

                workout.Exercises.Add(NewExercise);

                var workoutExercise = new LinkedWorkoutsToExercises
                                      {
                                          ExerciseId   = exerciseId
                                        , WorkoutId    = workoutId
                                        , LengthOfTime = NewExercise.LengthOfTime
                                      };

                var workoutExerciseId = DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise);

                if (workoutExerciseId == 0)
                {
                    throw new Exception("LinkedWorkoutsToExercises not added");
                }
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
