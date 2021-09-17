using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models.Intermediates
{
    [Table("ExerciseTypes")]
    public class ExerciseType
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Exercise))]
        public int ExerciseId { get; set; }

        [ForeignKey(typeof(TypeOfExercise))]
        public int TypeId { get; set; }
    }
}
