using System;
using System.Collections.Generic;
using System.Text;
using Avails.D_Flat.Extensions;
using PersonalTrainerWorkouts.Models.Intermediates;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models
{
    [Table("Exercises")]
    public class Exercise : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Description  { get; set; }
        public string LengthOfTime { get; set; }
        public int    Reps         { get; set; }

        public Exercise()
        {
            TypesOfExercise = new List<TypeOfExercise>();
            Equipment       = new List<Equipment>();
            Synergists      = new List<Synergist>();
        }

        [ManyToMany(typeof(WorkoutExercise)
                  , CascadeOperations = CascadeOperation.All)]
        public List<Workout> Workouts { get; set; }

        [ManyToMany(typeof(ExerciseType)
                  , CascadeOperations = CascadeOperation.All)]
        public List<TypeOfExercise> TypesOfExercise { get; set; }

        [ManyToMany(typeof(ExerciseEquipment)
                  , CascadeOperations = CascadeOperation.All)]
        public List<Equipment> Equipment { get; set; }

        [OneToMany]
        public List<Synergist> Synergists { get; set; }

        public override string ToString()
        {
            var text = new StringBuilder();
            text.Append($"\t{Name}");
            if (Reps > 0) text.Append($" ({nameof(Reps)}: {Reps})");
            if (LengthOfTime.HasValue() && LengthOfTime != "00:00") text.Append($" (Time: {LengthOfTime})");
            text.Append(Environment.NewLine);
            if (Description.HasValue()) text.Append(Description);

            return text.ToString();
        }
    }
}
