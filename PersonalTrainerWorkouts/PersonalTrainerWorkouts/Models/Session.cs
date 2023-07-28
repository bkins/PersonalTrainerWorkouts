using PersonalTrainerWorkouts.Models.ContactsAndClients;
using SQLite;

using SQLiteNetExtensions.Attributes;

using System;
using System.Collections.Generic;
using System.Text;
using Avails.D_Flat.Extensions;
using PersonalTrainerWorkouts.Models.Intermediates;

namespace PersonalTrainerWorkouts.Models
{
    [Table(nameof(Session))]
    public class Session : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate   { get; set; }

        public string   Note    { get; set; }
        public bool     Paid    { get; set; }

        [ForeignKey(typeof(Client))]
        public int ClientId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Client Client { get; set; }

        [ManyToMany(typeof(SessionWorkouts))]
        public List<Workout> Workouts { get; set; }

        public Session()
        {
            Client = new Client();
        }

        [Ignore]
        public string WorkoutsForDisplay => BuildWorkoutsForDisplay();

        public string BuildWorkoutsForDisplay()
        {
            var workoutString = new StringBuilder();

            foreach (var workout in Workouts)
            {
                workoutString.Append($"{workout.Name}; ");
            }

            var returnString = workoutString.ToString();
            if (returnString.HasValue())
            {
                //Remove the last ';'
                returnString = returnString.Remove(returnString.Length - 2, 1);
            }

            return returnString;
        }

        public override string ToString()
        {
            var sessionString = new StringBuilder();

            sessionString.Append($"  {StartDate.ToShortDateString()}");
            sessionString.Append(" ");
            sessionString.Append(StartDate.ToShortTimeString());
            sessionString.Append(" - ");
            sessionString.AppendLine(EndDate.ToShortTimeString());
            sessionString.AppendLine($"  {Client.DisplayName}");

            var workouts = BuildWorkoutsForDisplay();
            if (workouts.HasValue())
            {
                sessionString.AppendLine($"  {workouts}");
            }

            return sessionString.ToString();
        }
    }
}
