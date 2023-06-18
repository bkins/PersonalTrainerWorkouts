﻿using PersonalTrainerWorkouts.Models.ContactsAndClients;

using SQLite;

using SQLiteNetExtensions.Attributes;

using System;
using System.Collections.Generic;

namespace PersonalTrainerWorkouts.Models
{
    [Table(nameof(Session))]
    public class Session : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public string Note { get; set; }
        public bool Paid { get; set; }

        [ForeignKey(typeof(Client))]
        public int ClientId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Client Client { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Workout> Workouts { get; set; }
    }
}