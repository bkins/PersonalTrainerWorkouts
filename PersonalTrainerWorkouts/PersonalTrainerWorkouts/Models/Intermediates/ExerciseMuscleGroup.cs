using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models.Intermediates
{
    [Table("ExerciseMuscleGroups")]
    public partial class ExerciseMuscleGroup
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Exercise))]
        public int ExerciseId    { get; set; }

        [ForeignKey(typeof(MuscleGroup))]
        public int MuscleGroupId { get; set; }
        
    }
}
