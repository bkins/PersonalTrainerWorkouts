using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.Utilities;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ResolvedWorkoutExercise
    {
        public int                       Id                           { get; set; }
        public Workout                   Workout                      { get; set; }
        public Exercise                  Exercise                     { get; set; }
        public int                       LengthOfTime                 { get; set; }
        public int                       OrderBy                      { get; set; }
        public LinkedWorkoutsToExercises TheLinkedWorkoutsToExercises { get; set; }

        public ResolvedWorkoutExercise(int workoutExerciseId)
        {
            try
            {
                Id = workoutExerciseId;

                //var workoutExercise = App.AsyncDatabase.GetWorkoutExercisesById(Id).Result;

                TheLinkedWorkoutsToExercises = App.Database.GetLinkedWorkoutsToExercises(workoutExerciseId);

                //var workoutExercise = App.Database.GetWorkoutExercises()
                //                          .First(field => field.Id == Id);

                //Workout      = App.AsyncDatabase.GetWorkoutById(workoutExercise.WorkoutId).Result;
                Workout      = App.Database.GetWorkout(TheLinkedWorkoutsToExercises.WorkoutId);
                //Exercise     = App.AsyncDatabase.GetExercise(workoutExercise.ExerciseId).Result;
                Exercise     = App.Database.GetExercise(TheLinkedWorkoutsToExercises.ExerciseId);
                LengthOfTime = TheLinkedWorkoutsToExercises.LengthOfTime;
                OrderBy      = TheLinkedWorkoutsToExercises.OrderBy;
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message, Category.Error, e);
                throw;
            }
        }
    }
}
