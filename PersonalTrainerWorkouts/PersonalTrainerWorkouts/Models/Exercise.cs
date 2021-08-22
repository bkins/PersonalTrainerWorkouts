using System;
using System.Collections.Generic;
using PersonalTrainerWorkouts.Models.Intermediates;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Models
{
    [Table("Exercises")]
    public class Exercise : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //public string Name         { get; set; }
        public string Description  { get; set; }
        public int    LengthOfTime { get; set; }
        
        public Exercise()
        {
            TypesOfExercise = new List<TypeOfExercise>();
            Equipment       = new List<Equipment>();
            MuscleGroups    = new List<MuscleGroup>();
        }

        [ManyToMany( typeof(WorkoutExercise), CascadeOperations  = CascadeOperation.All)]
        public List<Workout> Workouts { get; set; }
        
        [ManyToMany( typeof(ExerciseMuscleGroup), CascadeOperations = CascadeOperation.All )]
        public List<MuscleGroup> MuscleGroups { get; set; }

        [ManyToMany(typeof(ExerciseType), CascadeOperations = CascadeOperation.All )]
        public List<TypeOfExercise> TypesOfExercise { get; set; }

        [ManyToMany( typeof(ExerciseEquipment), CascadeOperations = CascadeOperation.All )]
        public List<Equipment> Equipment { get; set; }
        
        
    }
}
