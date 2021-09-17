using System;
using System.Collections.Generic;
using PersonalTrainerWorkouts.Models.Intermediates;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models
{
    [Table("Workouts")]
    public class Workout : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string   Description    { get; set; }
        public int      Difficulty     { get; set; }
        public DateTime CreateDateTime { get; set; }

        public Workout()
        {
            Exercises = new List<Exercise>();
        }

        [ManyToMany(typeof(WorkoutExercise)
                  , CascadeOperations = CascadeOperation.All)]
        public List<Exercise> Exercises { get; set; }
    }
}
