using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models.Intermediates
{
    [Table("Synergists")]
    public class Synergist : IEquatable<Synergist>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Exercise))]
        public int ExerciseId { get; set; }

        [ForeignKey(typeof(MuscleGroup))]
        public int MuscleGroupId { get; set; }

        [ForeignKey(typeof(MuscleGroup))]
        public int OpposingMuscleGroupId { get; set; }

        public bool Equals(Synergist other)
        {
            if (ReferenceEquals(null
                              , other))
                return false;

            if (ReferenceEquals(this
                              , other))
                return true;

            return MuscleGroupId == other.MuscleGroupId && OpposingMuscleGroupId == other.OpposingMuscleGroupId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null
                              , obj))
                return false;

            if (ReferenceEquals(this
                              , obj))
                return true;

            if (obj.GetType() != this.GetType())
                return false;

            return Equals((Synergist)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (MuscleGroupId * 397) ^ OpposingMuscleGroupId;
            }
        }
    }
}
