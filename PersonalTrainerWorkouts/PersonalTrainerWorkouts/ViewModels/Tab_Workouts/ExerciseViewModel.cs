using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    public class ExerciseViewModel : ViewModelBase
    {
        public int      WorkoutExerciseId { get; set; }
        public Exercise Exercise          { get; set; }
        public string   Name              { get; set; }
        public string   Description       { get; set; }
        public string   LengthOfTime      { get; set; }
        public int      Reps              { get; set; }

        public ExerciseViewModel(int    workoutExerciseId
                               , int    exerciseId
                               , string lengthOfTime
                               , int    reps)
        {
            WorkoutExerciseId = workoutExerciseId;
            
            Exercise          = DataAccessLayer.GetExercise(exerciseId);
            Name              = Exercise.Name;
            Description       = Exercise.Description;
            LengthOfTime      = lengthOfTime;
            Reps              = reps;
        }
    }
}
