using System;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ExerciseLengthOfTime
    {
        public int       WorkoutExerciseId { get; set; }
        public Exercise Exercise          { get; set; }
        public int       LengthOfTime      { get; set; }

        public ExerciseLengthOfTime(int workoutExerciseId, int exerciseId, int lengthOfTime)
        {
            WorkoutExerciseId = workoutExerciseId;
            Exercise          = App.Database.GetExercise(exerciseId);
            LengthOfTime      = lengthOfTime;
        }
        //public ExerciseLengthOfTime(int workoutId, int exerciseId)
        //{
        //    Exercise     = App.DataStore.GetExercise(exerciseId);
        //    LengthOfTime = App.Database.GetExerciseLengthOfTime(workoutId, exerciseId).Result.LengthOfTime;
        //}

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
