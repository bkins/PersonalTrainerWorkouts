namespace PersonalTrainerWorkouts.Models
{
    public partial class ExerciseEquipment
    {
        public long Id { get; set; }
        public long ExerciseId { get; set; }
        public long EquipmentId { get; set; }

        public virtual Equipment Equipment { get; set; }
        public virtual Exercises Exercise { get; set; }
    }
}
