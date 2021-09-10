using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models
{
    [Obsolete("Being replaced with Synergists")]
    [Table("OpposingMuscleGroups")]
    public class OpposingMuscleGroup
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        //pseudo Foreign Keys
        public int  MuscleGroupId         { get; set; }
        public int  OpposingMuscleGroupId { get; set; }
        
    }
}
