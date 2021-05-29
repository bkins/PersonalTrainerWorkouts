namespace PersonalTrainerWorkouts.Models
{
    public partial class OpposingMuscleGroups
    {
        public long Id { get; set; }
        public long MuscleGroupId { get; set; }
        public long OpposingMuscleGroupId { get; set; }

        public virtual MuscleGroups MuscleGroup { get; set; }
        public virtual MuscleGroups OpposingMuscleGroup { get; set; }
    }
}
