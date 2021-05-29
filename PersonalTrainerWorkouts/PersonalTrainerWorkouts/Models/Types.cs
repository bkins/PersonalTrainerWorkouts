using System.Collections.Generic;

namespace PersonalTrainerWorkouts.Models
{
    public partial class Types
    {
        public Types()
        {
            ExerciseTypes = new HashSet<ExerciseTypes>();
        }

        public long Id { get; set; }
        public string Title { get; set; }

        public virtual ICollection<ExerciseTypes> ExerciseTypes { get; set; }
    }
}
