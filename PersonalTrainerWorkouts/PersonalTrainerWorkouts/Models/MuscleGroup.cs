using SQLite;

namespace PersonalTrainerWorkouts.Models
{
    [Table("MuscleGroups")]
    public class MuscleGroup : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //public MuscleGroup OpposingMuscleGroup { get; set; }

        //[ManyToMany(typeof(ExerciseMuscleGroup), CascadeOperations = CascadeOperation.All)]
        //public List<Exercise> Exercises { get; set; }

        //public MuscleGroup OpposingMuscleGroup { get; set; }
    }
}
