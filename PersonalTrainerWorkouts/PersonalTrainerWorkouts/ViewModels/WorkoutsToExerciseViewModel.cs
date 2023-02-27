using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ApplicationExceptions;
using Avails.D_Flat.Exceptions;
using Avails.D_Flat.Extensions;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class WorkoutsToExerciseViewModel : ViewModelBase
    {
        public Workout                                         Workout                         { get; set; }
        public ObservableCollection<LinkedWorkoutsToExercises> WorkoutsToExercises             { get; set; }
        public List<ExerciseViewModel>                         ExercisesWithIntermediateFields { get; set; }
        public List<ResolvedWorkoutsToExercise>                ResolvedWorkoutToExercisesList  { get; set; }
        public TimeSpan                                        TotalTime                       => GetTotalTime();
        public int                                             TotalReps                       => GetTotalReps();

        private TimeSpan GetTotalTime()
        {
            var total = new TimeSpan(0
                                   , 0
                                   , 0
                                   , 0);

            return ExercisesWithIntermediateFields.Select(exerciseLengthOfTime => exerciseLengthOfTime.LengthOfTime.ToTime())
                                                  .Aggregate(total
                                                           , (current
                                                            , exerciseLengthOfTimeAsTime) => current.Add(exerciseLengthOfTimeAsTime));
        }

        private int GetTotalReps()
        {
            return ExercisesWithIntermediateFields.Sum(exercise => exercise.Reps);
        }

        public WorkoutsToExerciseViewModel(string     workoutId
                                         , DataAccess dbAccessLayer)
        {
            DataAccessLayer = dbAccessLayer;
            Initialize(workoutId);
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
                Workout                        = DataAccessLayer.GetWorkout(int.Parse(workoutId));

                var orderListOfWorkoutsToExercises = new List<LinkedWorkoutsToExercises>(DataAccessLayer.GetLinkedWorkoutsToExercises(Workout.Id));

                WorkoutsToExercises             = new ObservableCollection<LinkedWorkoutsToExercises>(orderListOfWorkoutsToExercises.OrderBy(field => field.OrderBy));
                ExercisesWithIntermediateFields = new List<ExerciseViewModel>();

                BuildWorkoutToExerciseRelationship();
            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed to initialize the WorkoutsToExerciseViewModel"
                               , Category.Error
                               , e);
            }
        }

        private void BuildWorkoutToExerciseRelationship()
        {
            foreach (var workoutsToExercise in WorkoutsToExercises)
            {
                if (workoutsToExercise.ExerciseId == 0)
                    continue;

                var resolvedWorkoutsToExercise = new ResolvedWorkoutsToExercise(workoutsToExercise.Id);
                ResolvedWorkoutToExercisesList.Add(resolvedWorkoutsToExercise);

                var exerciseToAdd = DataAccessLayer.GetExercise(workoutsToExercise.ExerciseId);

                var lengthOfTimeToUse = GetLengthOfTimeToUse(exerciseToAdd
                                                           , workoutsToExercise).ToShortForm();

                var repsToUse = GetRepsToUse(exerciseToAdd
                                           , workoutsToExercise);

                ExercisesWithIntermediateFields.Add(new ExerciseViewModel(workoutsToExercise.Id
                                                                        , workoutsToExercise.ExerciseId
                                                                        , lengthOfTimeToUse
                                                                        , repsToUse));

                Workout.Exercises.Add(exerciseToAdd);
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

            if (lengthOfTimeFromWorkoutToExercises
             != new TimeSpan(0
                           , 0
                           , 0
                           , 0))
            {
                defaultLengthOfTime = lengthOfTimeFromWorkoutToExercises;
            }

            return defaultLengthOfTime;
        }

        public void SaveWorkoutsToExercise(LinkedWorkoutsToExercises workoutsToExercise)
        {
            if (workoutsToExercise.LengthOfTime.ToTime() > new TimeSpan(0
                                                                      , 1
                                                                      , 0
                                                                      , 0))
            {
                throw new ValueTooLargeException(nameof(workoutsToExercise.LengthOfTime)
                                               , workoutsToExercise.LengthOfTime
                                               , workoutsToExercise.LengthOfTime.GetType()
                                                                   .ToString()
                                               , workoutsToExercise.LengthOfTime
                                               , "it is greater than 1 hour.");
            }

            DataAccessLayer.UpdateLinkedWorkoutsToExercises(workoutsToExercise);
        }

        public int SaveWorkout()
        {
            if (Workout.Id == 0)
            {
                return DataAccessLayer.AddNewWorkout(Workout);
            }
            else
            {
                DataAccessLayer.UpdateWorkout(Workout);

                return Workout.Id;
            }
        }
    }
}
