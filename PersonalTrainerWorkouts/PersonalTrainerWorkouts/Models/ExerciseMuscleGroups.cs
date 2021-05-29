namespace PersonalTrainerWorkouts.Models
{
    public partial class ExerciseMuscleGroups
    {
        public long Id { get; set; }
        public long ExerciseId { get; set; }
        public long MuscleGroupId { get; set; }

        public virtual Exercises Exercise { get; set; }
        public virtual MuscleGroups MuscleGroup { get; set; }
    }
}
