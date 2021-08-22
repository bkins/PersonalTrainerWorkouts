using System.Collections.Generic;
using PersonalTrainerWorkouts.Models.Intermediates;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models
{
    [Table( "Types")]
    public class TypeOfExercise : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get;      set; }

        //public string Name { get; set; }
        
        [ManyToMany( typeof(ExerciseType), CascadeOperations = CascadeOperation.All)]
        public List<Exercise> Exercises { get; set; }

    }
}
