using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class WorkoutExerciseWithChildren : ViewModelBase
    {
        private Exercise                  _exercise;
        private LinkedWorkoutsToExercises _workoutExercise;
        private Workout                   _workout;
        private string                    _exerciseForDebugging;

        public Exercise Exercise
        {
            get => _exercise;
            set
            {
                SetValue(ref _exercise
                       , value);

                OnPropertyChanged();
            }
        }

        public LinkedWorkoutsToExercises WorkoutExercise
        {
            get => _workoutExercise;
            set
            {
                SetValue(ref _workoutExercise
                       , value);

                OnPropertyChanged();
            }
        }

        public Workout Workout
        {
            get => _workout;
            set
            {
                SetValue(ref _workout
                       , value);

                OnPropertyChanged();
            }
        }

        public string ExerciseForDebugging
        {
            get => _exerciseForDebugging;
            set
            {
                SetValue(ref _exerciseForDebugging
                       , value);

                OnPropertyChanged();
            }
        }

        public WorkoutExerciseWithChildren(int workoutId)
        {
            Workout = DataAccessLayer.GetWorkout(workoutId);
        }

        public void Save()
        {
            DataAccessLayer.UpdateLinkedWorkoutsToExercises(WorkoutExercise);
        }
    }
}
