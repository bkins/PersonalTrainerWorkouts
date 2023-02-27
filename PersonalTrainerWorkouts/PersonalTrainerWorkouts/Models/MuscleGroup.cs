using SQLite;

namespace PersonalTrainerWorkouts.Models
{
    [Table("MuscleGroups")]
    public class MuscleGroup : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //public MuscleGroup OpposingMuscleGroup { get; set; }

        public MuscleGroup()
        {
            //Exercises = new List<Exercise>();
        }

        //[ManyToMany(typeof(ExerciseMuscleGroup), CascadeOperations = CascadeOperation.All)]
        //public List<Exercise> Exercises { get; set; }

        //public MuscleGroup OpposingMuscleGroup { get; set; }
    }
}
