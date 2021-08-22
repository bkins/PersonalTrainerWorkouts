using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;

namespace PersonalTrainerWorkouts.Data
{
    public class DataStore
    {
        public AsyncDatabase WorkingAsyncDatabase { get; set; }

        public DataStore(AsyncDatabase asyncDatabase)
        {
            WorkingAsyncDatabase = asyncDatabase;
        }

        public List<Workout> GetAllWorkouts()
        {
            return WorkingAsyncDatabase.Workouts;
        }

        public List<Exercise> GetAllExercises()
        {
            return WorkingAsyncDatabase.Exercises;
        }

        public List<WorkoutExercise> GetAllWorkoutExercises()
        {
            return WorkingAsyncDatabase.WorkoutExercises;
        }

        public WorkoutExercise GetWorkoutExercise(int workoutId
                                                 , int exerciseId)
        {
            return GetAllWorkoutExercises()
                   .FirstOrDefault( workoutExercise => workoutExercise.WorkoutId == workoutId && workoutExercise.ExerciseId == exerciseId );
        }

        public Workout GetWorkout(int workoutId)
        {
            return GetAllWorkouts()
                   .FirstOrDefault( workout => workout.Id == workoutId );
        }

        public Exercise GetExercise(int exerciseId)
        {
            return GetAllExercises()
                   .FirstOrDefault( exercise => exercise.Id == exerciseId );
        }

        public void InsertWorkout(Workout workout)
        {
            WorkingAsyncDatabase.SaveWorkoutAsync( workout );
        }

        public void InsertWorkout(Workout  workout
                                , Exercise exercise)
        {
            workout.Exercises.Add( exercise );
            WorkingAsyncDatabase.SaveWorkoutAsync( workout );
        }

        public void InsertWorkout(Workout        workout
                                , List<Exercise> exercises)
        {
            workout.Exercises = exercises;
            WorkingAsyncDatabase.SaveWorkoutAsync( workout );
        }
    }
}
