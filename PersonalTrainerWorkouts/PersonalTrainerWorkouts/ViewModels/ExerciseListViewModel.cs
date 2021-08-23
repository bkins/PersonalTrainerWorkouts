using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.HelperModels;
using PersonalTrainerWorkouts.Models.Intermediates;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ExerciseListViewModel : ViewModelBase
    {
        private ObservableCollection<WorkoutExerciseWithChildren> _linkWorkoutExercises;
        public int                                               WorkoutId { get; set; }

        public ObservableCollection<WorkoutExerciseWithChildren> LinkWorkoutExercises
        {
            get => _linkWorkoutExercises;
            set
            {
                SetValue(ref _linkWorkoutExercises, value); 
                OnPropertyChanged(nameof(LinkWorkoutExercises));
            }
        }

        private static DataAccess DataAccessLayer => new DataAccess(App.Database);

        public ExerciseListViewModel(int workoutId)
        {
            WorkoutId = workoutId;
            RefreshData();
        }

        public void RefreshData()
        {
            LinkWorkoutExercises = new ObservableCollection<WorkoutExerciseWithChildren>();

            //var workoutExercisesAssociatedWithWorkout = DataAccessLayer.GetWorkoutExercises(_workoutId);
            var workoutExercisesAssociatedWithWorkout = DataAccessLayer.GetLinkedWorkoutsToExercises(WorkoutId);

            var workoutExercisesWithChildren = GetWorkoutExerciseWithChildrenFromDatabase(WorkoutId
                                                                                        , workoutExercisesAssociatedWithWorkout);

            foreach (var workoutExerciseWithChildren in workoutExercisesWithChildren)
            {
                LinkWorkoutExercises.Add(workoutExerciseWithChildren);
            }
            ReorderWorkoutExercises();
        }

        private IEnumerable<WorkoutExerciseWithChildren> 
                    GetWorkoutExerciseWithChildrenFromDatabase(int workoutId
                                                             , IEnumerable<LinkedWorkoutsToExercises> workoutExercisesAssociatedWithWorkout)
        {
            var exercisesAssociatedWithWorkout = workoutExercisesAssociatedWithWorkout.ToList();

            foreach (var newRecord in exercisesAssociatedWithWorkout
                                        .Select(workoutExercise => new WorkoutExerciseWithChildren(workoutId)
                                                                   {
                                                                       WorkoutExercise = workoutExercise
                                                                     , Exercise        = DataAccessLayer.GetExercise(workoutExercise.ExerciseId)
                                                                     , Workout         = DataAccessLayer.GetWorkout(workoutId)
                                                                   }))
            {
                newRecord.ExerciseForDebugging = $"{newRecord.Exercise.Name} ({newRecord.WorkoutExercise.OrderBy})";

                yield return newRecord;
            }



            //foreach (var workoutExercise in exercisesAssociatedWithWorkout)
            //    yield return new WorkoutExerciseWithChildren(workoutId)
            //                 {
            //                     WorkoutExercise = workoutExercise
            //                   , Exercise        = DataAccessLayer.GetExercise(workoutExercise.ExerciseId)
            //                   , Workout         = DataAccessLayer.GetWorkout(workoutId)
            //                 };
        }

        public void ReorderWorkoutExercises()
        {
            LinkWorkoutExercises = new ObservableCollection<WorkoutExerciseWithChildren>(LinkWorkoutExercises.OrderBy(field => field.WorkoutExercise.OrderBy));
        }
    }
}
