using System;
using System.Collections.Generic;
using System.Linq;
using Avails.D_Flat.Extensions;
using Java.Lang;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
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
        
        [ForeignKey(typeof(Session))]
        public int SessionId { get; set; }

        public string ToTextMessageString(Client client)
        {
            var textMessage = new StringBuilder();

            textMessage.Append($"Hi {client.DisplayName},");
            textMessage.Append(Environment.NewLine);
            textMessage.Append("Here is a workout for you:");
            textMessage.Append(Environment.NewLine);
            textMessage.Append($"{Name}:");
            textMessage.Append(Environment.NewLine);
            if (Description.HasValue()) textMessage.Append(Description);
            textMessage.Append(Environment.NewLine);

            if (! Exercises.Any()) return textMessage.ToString();

            foreach (var exercise in Exercises)
            {
                textMessage.Append(exercise.ToString());
            }

            return textMessage.ToString();
        }
    }
}
