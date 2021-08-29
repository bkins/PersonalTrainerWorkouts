using System;
using System.Linq;
using ApplicationExceptions;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.Utilities;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ExerciseAddEditViewModel  : ViewModelBase 
    {
        public Exercise Exercise     { get; set; }
        public Workout  Workout      { get; set; }
        public string   LengthOfTime { get; set; }

        public ExerciseAddEditViewModel()
        {
            Workout  = new Workout();
            Exercise = new Exercise();
        }

        public ExerciseAddEditViewModel(int workoutId, int exerciseId)
        {
            Workout  = DataAccessLayer.GetWorkout(workoutId);
            Exercise = Workout.Exercises.FirstOrDefault(e => e.Id == exerciseId) ?? new Exercise();

            LengthOfTime = Exercise?.LengthOfTime.ToTime()
                                    .ToShortForm();
        }
        
        public void UpdateExercise()
        {
            DataAccessLayer.UpdateExercise(Exercise);
        }

        public void SaveExercise(int workoutId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Exercise.Name))
                    throw new UnnamedEntityException($"{nameof(Exercise)} was not named.  Must be named before attempting to save.");
                
                AddExerciseToWorkout(workoutId);

                var workoutExercise = CreateNewWorkoutsToExercise(workoutId);

                var workoutExerciseId = DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise);

                ValidateWorkoutsToExerciseWasAdded(workoutExerciseId);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message, Category.Error, e);

                throw;
            }
        }

        private LinkedWorkoutsToExercises CreateNewWorkoutsToExercise(int workoutId)
        {
            var exerciseId = DataAccessLayer.AddNewExercise(Exercise);

            var workoutExercise = new LinkedWorkoutsToExercises
                                  {
                                      ExerciseId   = exerciseId
                                    , WorkoutId    = workoutId
                                    , LengthOfTime = Exercise.LengthOfTime
                                  };

            return workoutExercise;
        }

        private void AddExerciseToWorkout(int workoutId)
        {
            var workout = DataAccessLayer.GetWorkout(workoutId);
            workout.Exercises.Add(Exercise);
        }

        private static void ValidateWorkoutsToExerciseWasAdded(int workoutExerciseId)
        {
            if (workoutExerciseId == 0)
            {
                throw new Exception("LinkedWorkoutsToExercises not added");
            }
        }
    }
}