using System.Collections.Generic;

namespace PersonalTrainerWorkouts.Models
{
    public partial class MuscleGroups
    {
        public MuscleGroups()
        {
            ExerciseMuscleGroups = new HashSet<ExerciseMuscleGroups>();
            OpposingMuscleGroupsMuscleGroup = new HashSet<OpposingMuscleGroups>();
            OpposingMuscleGroupsOpposingMuscleGroup = new HashSet<OpposingMuscleGroups>();
        }

        public long Id { get; set; }
        public string Title { get; set; }

        public virtual ICollection<ExerciseMuscleGroups> ExerciseMuscleGroups { get; set; }
        public virtual ICollection<OpposingMuscleGroups> OpposingMuscleGroupsMuscleGroup { get; set; }
        public virtual ICollection<OpposingMuscleGroups> OpposingMuscleGroupsOpposingMuscleGroup { get; set; }
    }
}
