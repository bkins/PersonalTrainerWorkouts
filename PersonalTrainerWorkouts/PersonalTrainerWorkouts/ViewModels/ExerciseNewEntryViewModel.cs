using System;
using ApplicationExceptions;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.Utilities;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ExerciseNewEntryViewModel : ViewModelBase
    {
        public Exercise NewExercise { get; set; }
        
        private static DataAccess _dataAccess;
        
        private static DataAccess DataAccessLayer => _dataAccess = _dataAccess ?? new DataAccess(App.Database);

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

                var workoutExercise = new LinkedWorkoutsToExercises //WorkoutExercise
                                      {
                                          ExerciseId   = exerciseId
                                        , WorkoutId    = workoutId
                                        , LengthOfTime = NewExercise.LengthOfTime
                                      };
                
                //var workoutExerciseId = DataAccessLayer.AddWorkoutExercise(workoutExercise);
                var workoutExerciseId = DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise);

                if (workoutExerciseId==0)
                {
                    throw new Exception("LinkedWorkoutsToExercises not added");
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message, Category.Error, e);

                throw;
            }
        }
    }
}
