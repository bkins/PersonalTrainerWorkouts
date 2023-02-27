using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models
{
    [Table(nameof(Measurable))]
    public class Measurable
    {
        /*
         * * int      ClientId (FK)
         * * string   Variable (ex: waist, upper arms, but also bench press, dead lift, push ups)
         * * double   Value
         * * string   Unit of Measurement (inches, pounds, reps)
         * * DateTime Date taken
         * * string   Type (Measurement or Max) (consider a Type table and this column would hold the TypeId.)
         */

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string   Variable          { get; set; }
        public double   Value             { get; set; }
        public string   UnitOfMeasurement { get; set; }
        public DateTime DateTaken         { get; set; }
        public string   Type              { get; set; }
        
        [ForeignKey(typeof(Client))]
        public int ClientId { get; set; }
    }
}