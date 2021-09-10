using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models.Intermediates
{
    [Table("Synergists")]
    public class Synergist
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Exercise))]
        public int ExerciseId { get; set; }

        [ForeignKey(typeof(MuscleGroup))]
        public int MuscleGroupId { get; set; }
        
        [ForeignKey(typeof(MuscleGroup))]
        public int OpposingMuscleGroupId { get; set; }

    }
}
