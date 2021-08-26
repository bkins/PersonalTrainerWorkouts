using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationExceptions;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.Utilities;
using Xamarin.Forms;
namespace PersonalTrainerWorkouts.ViewModels
{
    public class ExerciseAddEditViewModel  : ViewModelBase 
    {
        private static DataAccess DataAccessLayer => new DataAccess(App.Database);
        
        public Exercise Exercise     { get; set; }
        public Workout  Workout      { get; set; }
        public string LengthOfTime { get; set; }

        public ExerciseAddEditViewModel()
        {
            Workout  = new Workout();
            Exercise = new Exercise();

        }

        public ExerciseAddEditViewModel(int workoutId, int exerciseId)
        {
            Workout  = DataAccessLayer.GetWorkout(workoutId);
            Exercise = Workout.Exercises.FirstOrDefault(e => e.Id == exerciseId);

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
                
                var workout    = DataAccessLayer.GetWorkout(workoutId);
                var exerciseId = DataAccessLayer.AddNewExercise(Exercise);
                
                workout.Exercises.Add(Exercise);

                var workoutExercise = new LinkedWorkoutsToExercises 
                                      {
                                          ExerciseId   = exerciseId
                                        , WorkoutId    = workoutId
                                        , LengthOfTime = Exercise.LengthOfTime
                                      };
                
                var workoutExerciseId = DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise);

                if (workoutExerciseId ==0)
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