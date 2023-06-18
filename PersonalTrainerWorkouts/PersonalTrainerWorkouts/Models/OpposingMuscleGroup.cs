using System;

using SQLite;

namespace PersonalTrainerWorkouts.Models
{
    [Obsolete("Being replaced with Synergists", false)]
    [Table("OpposingMuscleGroups")]
    public class OpposingMuscleGroup
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //pseudo Foreign Keys
        public int MuscleGroupId { get; set; }
        public int OpposingMuscleGroupId { get; set; }
    }
}
