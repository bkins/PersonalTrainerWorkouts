using System.ComponentModel;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.ViewModels;

namespace PersonalTrainerWorkouts.Models.HelperModels
{
    public class WorkoutExerciseWithChildren : ViewModelBase
    {
        private Exercise                  _exercise;
        //private WorkoutExercise _workoutExercise;
        private LinkedWorkoutsToExercises _workoutExercise;
        private Workout                   _workout;
        private string                    _exerciseForDebugging;

        public Exercise Exercise
        {
            get => _exercise;
            set
            {
                SetValue(ref _exercise, value); 
                OnPropertyChanged(nameof(Exercise));
            }
        }

        //public WorkoutExercise WorkoutExercise
        public LinkedWorkoutsToExercises WorkoutExercise
        {
            get => _workoutExercise;
            set
            {
                SetValue(ref _workoutExercise, value);
                OnPropertyChanged(nameof(WorkoutExercise));
            }
        }

        public Workout Workout
        {
            get => _workout;
            set
            {
                SetValue(ref _workout, value); 
                OnPropertyChanged(nameof(Workout));
            }
        }

        public string ExerciseForDebugging
        {
            get => _exerciseForDebugging;
            set
            {
                SetValue(ref _exerciseForDebugging, value); 
                OnPropertyChanged(nameof(ExerciseForDebugging));
            }
        }

        private static DataAccess      DataAccessLayer      => new DataAccess(App.Database);

        public WorkoutExerciseWithChildren(int workoutId)
        {
            Workout = DataAccessLayer.GetWorkout(workoutId);

        }
        public void Save()
        {
            //DataAccessLayer.UpdateWorkoutExercise(WorkoutExercise);
            DataAccessLayer.UpdateLinkedWorkoutsToExercises(WorkoutExercise);
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        
        //public void OnPropertyChanged(string name)
        //{
        //    if (this.PropertyChanged != null)
        //        this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        //}

    }
}
