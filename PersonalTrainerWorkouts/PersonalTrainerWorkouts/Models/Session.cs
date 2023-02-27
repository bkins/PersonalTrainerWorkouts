using System;
using System.Collections.Generic;
using Windows.Web.AtomPub;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models
{
    [Table(nameof(Session))]
    public class Session : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public DateTime Date { get; set; }
        public string   Note { get; set; }
        
        [ForeignKey(typeof(Client))]
        public int ClientId { get; set; }
        
        [OneToOne]
        public Client Client { get; set; }
        
        [OneToMany]
        public List<Workout> Workouts { get; set; }
    }
}