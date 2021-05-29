using System.Collections.Generic;

namespace PersonalTrainerWorkouts.Models
{
    public partial class Equipment
    {
        public Equipment()
        {
            ExerciseEquipment = new HashSet<ExerciseEquipment>();
        }

        public long Id { get; set; }
        public string Title { get; set; }

        public virtual ICollection<ExerciseEquipment> ExerciseEquipment { get; set; }
    }
}
