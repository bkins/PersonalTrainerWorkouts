using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Models
{
    [Table("Workouts")]
    public partial class Workouts
    {
        public Workouts()
        {
            //WorkoutExercises = new HashSet<WorkoutExercises>();
            Exercises = new List<Exercises>();
        }

        [PrimaryKey, AutoIncrement]
        public int Id 
        { get; 
            set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Difficulty { get; set; }
        public DateTime CreateDateTime { get; set; }
        
        [ManyToMany(typeof(WorkoutExercises), CascadeOperations = CascadeOperation.CascadeRead)] 
        public List<Exercises> Exercises { get; set; }

        // public virtual ICollection<WorkoutExercises> WorkoutExercises { get; set; }
    }
}
