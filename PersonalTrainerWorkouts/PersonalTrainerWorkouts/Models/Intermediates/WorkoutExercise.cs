using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models.Intermediates
{
    [Table("WorkoutExercises")]
    public class WorkoutExercise
    {
        [PrimaryKey, AutoIncrement]
        public int Id           { get; set; }

        public int LengthOfTime { get; set; }
        public int OrderBy      { get; set; }

        [ForeignKey(typeof(Workout))]
        public int WorkoutId    { get; set; }

        [ForeignKey(typeof(Exercise))]
        public int ExerciseId   { get; set; }
        
    }
}
