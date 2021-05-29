using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Models
{
    [Table("Exercises")]
    public class Exercises
    {
        public Exercises()
        {
            //ExerciseEquipment = new HashSet<ExerciseEquipment>();
            //ExerciseMuscleGroups = new HashSet<ExerciseMuscleGroups>();
            //ExerciseTypes = new HashSet<ExerciseTypes>();
            //WorkoutExercises = new HashSet<WorkoutExercises>();
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? LengthOfTime { get; set; }
        public string PushPull { get; set; }
        
        [ManyToMany(typeof(Workouts), CascadeOperations  = CascadeOperation.CascadeRead)]
        public List<Workouts> Workouts { get; set; }

        public string DisplayText => Name;

        public override string ToString()
        {
            return Name;
        }
        //public virtual ICollection<ExerciseEquipment> ExerciseEquipment { get; set; }
        //public virtual ICollection<ExerciseMuscleGroups> ExerciseMuscleGroups { get; set; }
        //public virtual ICollection<ExerciseTypes> ExerciseTypes { get; set; }
        //public virtual ICollection<WorkoutExercises> WorkoutExercises { get; set; }
    }
}
