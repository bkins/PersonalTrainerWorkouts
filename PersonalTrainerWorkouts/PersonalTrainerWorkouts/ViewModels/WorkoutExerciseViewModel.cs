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
        //public List<WorkoutExercise>         WorkoutExercises          { get; set; }
        public Workout                         Workout                   { get; set; }
        public List<Exercise>                  Exercises                 { get; set; }
        public List<LinkedWorkoutsToExercises> WorkoutsToExercises       { get; set; }
        public List<ExerciseLengthOfTime>      ExercisesWithLengthOfTime { get; set; }
        public List<ResolvedWorkoutExercise>   WorkoutExerciseList       { get; set; }

        public int TotalTime => GetTotalTime();

        private int GetTotalTime()
        {
            var total = 0;
            foreach (var exerciseLengthOfTime in ExercisesWithLengthOfTime)
            {
                total += exerciseLengthOfTime.LengthOfTime;
            }

            return total;
        }

        public WorkoutExerciseViewModel(string workoutId)
        {
            Initialize(workoutId);
        }

        private async void Initialize(string workoutId)
        {
            try
            {
                WorkoutExerciseList = new List<ResolvedWorkoutExercise>();
                //Workout             = await App.AsyncDatabase.GetWorkoutsAsync(workoutId) ?? new Workout();
                Workout             = App.Database.GetWorkout(int.Parse(workoutId));
                
                //WorkoutExercises = App.AsyncDatabase.GetAllWorkoutExercisesByWorkout(Workout.Id).Result ?? new List<WorkoutExercise>();

                WorkoutsToExercises = new List<LinkedWorkoutsToExercises>
                                      (App.Database.GetAllLinkedWorkoutsToExercises()
                                          .Where(field => field.WorkoutId == Workout.Id)
                                      ).OrderBy(field => field.OrderBy)
                                       .ToList();

                //WorkoutExercises = new List<WorkoutExercise>
                //                        (App.Database.GetWorkoutExercises()
                //                                     .Where(field => field.WorkoutId == Workout.Id)
                //                        ).OrderBy(field => field.OrderBy)
                //                         .ToList();
            
                ExercisesWithLengthOfTime = new List<ExerciseLengthOfTime>();

                foreach (var workoutsToExercise in WorkoutsToExercises)
                {
                    if (workoutsToExercise.ExerciseId != 0)
                    {
                        var resolvedWorkoutsToExercise = new ResolvedWorkoutExercise(workoutsToExercise.Id);
                        WorkoutExerciseList.Add(resolvedWorkoutsToExercise);

                        var exerciseToAdd           = App.Database.GetExercise( workoutsToExercise.ExerciseId );
                        var lengthOfTimeToUse       = exerciseToAdd.LengthOfTime;

                        if (workoutsToExercise.LengthOfTime != 0)
                        {
                            lengthOfTimeToUse = workoutsToExercise.LengthOfTime;
                        }

                        ExercisesWithLengthOfTime.Add(new ExerciseLengthOfTime(workoutsToExercise.Id
                                                                             , workoutsToExercise.ExerciseId
                                                                             , lengthOfTimeToUse));
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
            
            //BENDO: It appears to be clear that the exerciseId is not needed.  And thus, this overloaded Initialize is not needed either. 
            return;

            var workoutExercises     = App.Database.GetWorkoutExercises();
            var foundWorkoutExercise = workoutExercises.Where
                                                        (
                                                         item => item.WorkoutId  == Convert.ToInt32( workoutId )
                                                              && item.ExerciseId == Convert.ToInt32( exerciseId )
                                                        );

        }
    }
}
