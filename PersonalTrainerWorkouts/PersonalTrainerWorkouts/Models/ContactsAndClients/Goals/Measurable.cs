using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models.ContactsAndClients.Goals
{
    [Table($"{nameof(Measurable)}s")]
    public class Measurable : IEquatable<Measurable>
    {
        /*
         * * int      Id (PK)
         * * int      ClientId (FK)
         * * string   Variable (ex: waist, upper arms, but also bench press, dead lift, push ups)
         * * double   Value
         * * string   Unit of Measurement (inches, pounds, reps, minutes (seconds?))
         * * DateTime Date taken
         * * string   Type (Measurement or Max) (consider a Type table and this column would hold the TypeId.)
         */

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string   Variable       { get; set; }
        public double   Value          { get; set; }
        public DateTime DateTaken      { get; set; }
        /// <summary>
        /// Measure or Max //TODO: Make a table of these values
        /// </summary>
        public string   Type           { get; set; } 
        /// <summary>
        /// Baseline, Target, Internal                                      
        /// Baseline: The beginning measurement when Goal is set            
        /// Target:   The measurement desired (defines when the Goal is met)
        /// Interim: The measurements between the Baseline and Target
        /// When an Interim Measurable equals the Target Measurable, the Gaol's DateCompleted is set 
        /// </summary>
        public Succession GoalSuccession { get; set; } 
        
        [ForeignKey(typeof(Client))]
        public int ClientId { get; set; }

        public string UnitOfMeasurement { get; set; }
        
        [ForeignKey(typeof(Goal))]
        public int GoalId { get; set; }
        
        // [ForeignKey(typeof(Measurable))]
        // public int PreviousMeasurementId { get; set; }

        public override string ToString()
        {
            return $"{DateTaken.ToShortDateString()} : {GoalSuccession} - {Value} {UnitOfMeasurement}"; //TODO: add UnitOfMeasurement when implemented
        }

        public bool Equals(Measurable other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Id                  == other.Id
                && Variable            == other.Variable
                && Type                == other.Type
                && ClientId            == other.ClientId
                && UnitOfMeasurement   == other.UnitOfMeasurement
                && GoalId              == other.GoalId
                && Value.Equals(other.Value)
                && DateTaken.Equals(other.DateTaken);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            
            return obj.GetType() == GetType() 
                && Equals((Measurable)obj);

        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Variable != null ? Variable.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Value.GetHashCode();
                hashCode = (hashCode * 397) ^ DateTaken.GetHashCode();
                hashCode = (hashCode * 397) ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ClientId;
                hashCode = (hashCode * 397) ^ (UnitOfMeasurement != null ? UnitOfMeasurement.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ GoalId;

                return hashCode;
            }
        }
    }
}
