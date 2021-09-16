using SQLite;

namespace PersonalTrainerWorkouts.Models.Intermediates
{
    [Table("LinkedWorkoutsToExercises")]
    public class LinkedWorkoutsToExercises
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //pseudo Foreign Keys
        public int WorkoutId  { get; set; }
        public int ExerciseId { get; set; }

        public string LengthOfTime { get; set; }
        public int    Reps         { get; set; }
        public int    OrderBy      { get; set; }
    }
}
