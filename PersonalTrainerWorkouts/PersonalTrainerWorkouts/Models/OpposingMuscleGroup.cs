using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models
{
    [Table("OpposingMuscleGroups")]
    public class OpposingMuscleGroup
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(MuscleGroup))]
        public int  MuscleGroupId         { get; set; }
        
        [ForeignKey(typeof(MuscleGroup))]
        public int  OpposingMuscleGroupId { get; set; }
        
    }
}
