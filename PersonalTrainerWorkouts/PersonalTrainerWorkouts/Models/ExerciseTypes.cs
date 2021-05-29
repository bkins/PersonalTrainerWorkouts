namespace PersonalTrainerWorkouts.Models
{
    public partial class ExerciseTypes
    {
        public long Id { get; set; }
        public long ExerciseId { get; set; }
        public long TypeId { get; set; }

        public virtual Exercises Exercise { get; set; }
        public virtual Types Type { get; set; }
    }
}
