using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models.Intermediates
{
    [Table("ExerciseMuscleGroups")]
    public class ExerciseMuscleGroup
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Exercise))]
        public int ExerciseId    { get; set; }

        [ForeignKey(typeof(MuscleGroup))]
        public int MuscleGroupId { get; set; }
        
    }
}
