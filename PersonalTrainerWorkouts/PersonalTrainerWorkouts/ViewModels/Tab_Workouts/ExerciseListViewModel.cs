using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PersonalTrainerWorkouts.Models.Intermediates;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    public class ExerciseListViewModel : ViewModelBase
    {
        public int WorkoutId { get; set; }

        private ObservableCollection<WorkoutExerciseWithChildren> _linkWorkoutExercises;

        public ObservableCollection<WorkoutExerciseWithChildren> LinkWorkoutExercises
        {
            get => _linkWorkoutExercises;
            set
            {
                SetValue(ref _linkWorkoutExercises
                       , value);

                OnPropertyChanged();
            }
        }

        public ExerciseListViewModel(int workoutId)
        {
            WorkoutId = workoutId;
            RefreshData();
        }

        public void RefreshData()
        {
            LinkWorkoutExercises = new ObservableCollection<WorkoutExerciseWithChildren>();

            var workoutExercisesAssociatedWithWorkout = DataAccessLayer.GetLinkedWorkoutsToExercises(WorkoutId);

            var workoutExercisesWithChildren = GetWorkoutExerciseWithChildren(WorkoutId
                                                                            , workoutExercisesAssociatedWithWorkout);

            foreach (var workoutExerciseWithChildren in workoutExercisesWithChildren)
            {
                LinkWorkoutExercises.Add(workoutExerciseWithChildren);
            }

            ReorderWorkoutExercises();
        }

        private IEnumerable<WorkoutExerciseWithChildren> GetWorkoutExerciseWithChildren(int                                    workoutId
                                                                                      , IEnumerable<LinkedWorkoutsToExercises> workoutExercisesAssociatedWithWorkout)
        {
            var exercisesAssociatedWithWorkout = workoutExercisesAssociatedWithWorkout.ToList();

            foreach (var newRecord in exercisesAssociatedWithWorkout.Select(workoutExercise => new WorkoutExerciseWithChildren(workoutId)
                                                                                               {
                                                                                                   WorkoutExercise = workoutExercise
                                                                                                 , Exercise        = DataAccessLayer.GetExercise(workoutExercise.ExerciseId)
                                                                                                 , Workout         = DataAccessLayer.GetWorkout(workoutId)
                                                                                               }))
            {
                newRecord.ExerciseForDebugging = $"{newRecord.Exercise.Name} ({newRecord.WorkoutExercise.OrderBy})";

                yield return newRecord;
            }
        }

        public void ReorderWorkoutExercises()
        {
            LinkWorkoutExercises = new ObservableCollection<WorkoutExerciseWithChildren>(LinkWorkoutExercises.OrderBy(field => field.WorkoutExercise.OrderBy));
        }
    }
}
