using System;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ExerciseViewModel
    {
        private static DataAccess DataAccessLayer => new DataAccess(App.Database);

        public int      WorkoutExerciseId { get; set; }
        public Exercise Exercise          { get; set; }
        public string   Name              { get; set; }
        public string   Description       { get; set; }
        public int      LengthOfTime      { get; set; }
        public int      Reps              { get; set; }

        public ExerciseViewModel()
        {

        }

        public ExerciseViewModel(int workoutExerciseId, int exerciseId, int lengthOfTime, int reps)
        {
            WorkoutExerciseId = workoutExerciseId;
            Exercise          = DataAccessLayer.GetExercise(exerciseId);
            Name              = Exercise.Name;
            Description       = Exercise.Description;
            LengthOfTime      = lengthOfTime;
            Reps              = reps;
        }
        
        private TimeSpan FormattedLengthAsTime()
        {
            var minutesAsTicks   = TimeSpan.FromMinutes( LengthOfTime ).Ticks;
            var timeSpanOfLength = new TimeSpan( minutesAsTicks );

            return timeSpanOfLength;//.ToString( "hh:mm" );
        }

        private string ConvertMinutes( TimeSpan interval )
        {
            
            string intervalStr = interval.ToString( );
            int    pointIndex  = intervalStr.IndexOf( ':' );

            pointIndex = intervalStr.IndexOf( '.', pointIndex );
            if( pointIndex < 0 ) intervalStr += "        ";
            return intervalStr;
        } 
    }
}
