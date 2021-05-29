using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models
{
    [Table("WorkoutExercises")]
    public partial class WorkoutExercises
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        //public long WorkoutId { get; set; }
        //public long ExerciseId { get; set; }

        //public virtual Exercises Exercise { get; set; }
        //public virtual PersonalTrainerWorkouts.Models.Workouts Workout { get; set; }

        [ForeignKey(typeof(Workouts))] 
        public int WorkoutId { get; set; }
        
        [ForeignKey(typeof(Exercises))] 
        public int ExerciseId { get; set; }
    }
}
