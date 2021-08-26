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
    public class WorkoutsToExerciseViewModel : ViewModelBase
    {
        private static DataAccess DataAccessLayer => new DataAccess(App.Database);

        public Workout                          Workout                         { get; set; }
        public List<LinkedWorkoutsToExercises>  WorkoutsToExercises             { get; set; }
        public List<ExerciseViewModel>          ExercisesWithIntermediateFields { get; set; }
        public List<ResolvedWorkoutsToExercise> ResolvedWorkoutToExercisesList  { get; set; }
        public TimeSpan                         TotalTime                       => GetTotalTime();
        public int                              TotalReps                       => GetTotalReps();

        private TimeSpan GetTotalTime()
        {
            var total = new TimeSpan(0, 0, 0, 0);

            foreach (var exerciseLengthOfTime in ExercisesWithIntermediateFields)
            {
                var exerciseLengthOfTimeAsTime = exerciseLengthOfTime.LengthOfTime.ToTime();
                total =  total.Add(exerciseLengthOfTimeAsTime);
            }

            return total;
        }

        private int GetTotalReps()
        {
            var total = 0;

            foreach (var exercise in ExercisesWithIntermediateFields)
            {
                total += exercise.Reps;
            }

            return total;
        }

        public WorkoutsToExerciseViewModel(string workoutId)
        {
            Initialize(workoutId);
            
        }

        private void Initialize(string workoutId)
        {
            try
            {
                ResolvedWorkoutToExercisesList = new List<ResolvedWorkoutsToExercise>();
                Workout             = DataAccessLayer.GetWorkout(int.Parse(workoutId));

                var orderListOfWorkoutsToExercises = new List<LinkedWorkoutsToExercises>(DataAccessLayer.GetLinkedWorkoutsToExercises(Workout.Id));
                WorkoutsToExercises = orderListOfWorkoutsToExercises.OrderBy(field => field.OrderBy)
                                                                    .ToList();
                
                ExercisesWithIntermediateFields = new List<ExerciseViewModel>();

                foreach (var workoutsToExercise in WorkoutsToExercises)
                {
                    if (workoutsToExercise.ExerciseId == 0)
                        continue;

                    var resolvedWorkoutsToExercise = new ResolvedWorkoutsToExercise(workoutsToExercise.Id);
                    ResolvedWorkoutToExercisesList.Add(resolvedWorkoutsToExercise);

                    var exerciseToAdd       = App.Database.GetExercise( workoutsToExercise.ExerciseId );

                    var lengthOfTimeToUse = GetLengthOfTimeToUse(exerciseToAdd, workoutsToExercise).ToShortForm(); //.ToString(Constants.TimeFormat);
                    var repsToUse           = GetRepsToUse(exerciseToAdd
                                                         , workoutsToExercise);

                    ExercisesWithIntermediateFields.Add(new ExerciseViewModel(workoutsToExercise.Id
                                                                            , workoutsToExercise.ExerciseId
                                                                            , lengthOfTimeToUse
                                                                            , repsToUse));
                    Workout.Exercises.Add(exerciseToAdd);
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed to initialize the WorkoutsToExerciseViewModel", Category.Error, e);
            }
        }

        private static int GetRepsToUse(Exercise                  exerciseToAdd
                                      , LinkedWorkoutsToExercises workoutsToExercise)
        {
            var repsToUse = exerciseToAdd.Reps;

            if (workoutsToExercise.Reps != 0)
            {
                repsToUse = workoutsToExercise.Reps;
            }

            return repsToUse;
        }

        private static TimeSpan GetLengthOfTimeToUse(Exercise                  exerciseToAdd
                                                   , LinkedWorkoutsToExercises workoutsToExercise)
        {
            var defaultLengthOfTime                = exerciseToAdd.LengthOfTime.ToTime();
            var lengthOfTimeFromWorkoutToExercises = workoutsToExercise.LengthOfTime.ToTime();

            if (lengthOfTimeFromWorkoutToExercises != new TimeSpan(0, 0, 0, 0))
            {
                defaultLengthOfTime = lengthOfTimeFromWorkoutToExercises;
            }

            return defaultLengthOfTime;
        }

        public WorkoutsToExerciseViewModel(string workoutId
                                         , string exerciseId)
        {
            Initialize( workoutId, exerciseId );
        }

        private void Initialize(string workoutId
                                    , string exerciseId)
        {
            Initialize(workoutId);
        }

        public void Save(LinkedWorkoutsToExercises workoutsToExercise)
        {
            if (workoutsToExercise.LengthOfTime.ToTime() > new TimeSpan(0, 1, 0, 0))
            {
                throw new ValueTooLargeException(nameof(workoutsToExercise.LengthOfTime)
                                               , workoutsToExercise.LengthOfTime
                                               , workoutsToExercise.LengthOfTime.GetType().ToString()
                                               , workoutsToExercise.LengthOfTime
                                               , "it is great than 1 hour.");
            }

            DataAccessLayer.UpdateLinkedWorkoutsToExercises(workoutsToExercise);
        }
    }
}
