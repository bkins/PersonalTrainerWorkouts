using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.Utilities;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class WorkoutExerciseViewModel : ViewModelBase
    {
        public Workout                         Workout                   { get; set; }
        public List<Exercise>                  Exercises                 { get; set; }
        public List<LinkedWorkoutsToExercises> WorkoutsToExercises       { get; set; }
        public List<ExerciseLengthOfTime>      ExercisesWithLengthOfTime { get; set; }
        public List<ResolvedWorkoutExercise>   WorkoutExerciseList       { get; set; }
        public int                             TotalTime                 => GetTotalTime();
        public int                             TotalReps                 => GetTotalReps();

        private int GetTotalTime()
        {
            var total = 0;

            foreach (var exerciseLengthOfTime in ExercisesWithLengthOfTime)
            {
                total += exerciseLengthOfTime.LengthOfTime;
            }

            return total;
        }

        private int GetTotalReps()
        {
            var total = 0;

            foreach (var exercise in ExercisesWithLengthOfTime)
            {
                total += exercise.Reps;
            }

            return total;
        }

        public WorkoutExerciseViewModel(string workoutId)
        {
            Initialize(workoutId);
        }

        private void Initialize(string workoutId)
        {
            try
            {
                WorkoutExerciseList = new List<ResolvedWorkoutExercise>();
                Workout             = App.Database.GetWorkout(int.Parse(workoutId));
                
                WorkoutsToExercises = new List<LinkedWorkoutsToExercises>
                                      (App.Database.GetAllLinkedWorkoutsToExercises()
                                          .Where(field => field.WorkoutId == Workout.Id)
                                      ).OrderBy(field => field.OrderBy)
                                       .ToList();
                
                ExercisesWithLengthOfTime = new List<ExerciseLengthOfTime>();

                foreach (var workoutsToExercise in WorkoutsToExercises)
                {
                    if (workoutsToExercise.ExerciseId != 0)
                    {
                        var resolvedWorkoutsToExercise = new ResolvedWorkoutExercise(workoutsToExercise.Id);
                        WorkoutExerciseList.Add(resolvedWorkoutsToExercise);

                        var exerciseToAdd     = App.Database.GetExercise( workoutsToExercise.ExerciseId );
                        var lengthOfTimeToUse = exerciseToAdd.LengthOfTime;
                        var reps              = exerciseToAdd.Reps;

                        if (workoutsToExercise.LengthOfTime != 0)
                        {
                            lengthOfTimeToUse = workoutsToExercise.LengthOfTime;
                        }

                        ExercisesWithLengthOfTime.Add(new ExerciseLengthOfTime(workoutsToExercise.Id
                                                                             , workoutsToExercise.ExerciseId
                                                                             , lengthOfTimeToUse
                                                                             , reps));
                        Workout.Exercises.Add(exerciseToAdd);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed to initialize the WorkoutExerciseViewModel", Category.Error, e);
            }
        }

        public WorkoutExerciseViewModel(string workoutId
                                      , string exerciseId)
        {
            Initialize( workoutId, exerciseId );
        }

        private void Initialize(string workoutId
                                    , string exerciseId)
        {
            Initialize(workoutId);
        }
    }
}
