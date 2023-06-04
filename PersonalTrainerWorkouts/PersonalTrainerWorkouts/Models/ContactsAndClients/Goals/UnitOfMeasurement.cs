using SQLite;

namespace PersonalTrainerWorkouts.Models.ContactsAndClients.Goals
{
    [Table($"{nameof(UnitOfMeasurement)}s")]
    internal class UnitOfMeasurement
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public Unit Unit { get; set; }
    }

    public enum Unit
    {
        Inches
      , Pounds
      , Reps
      , Minutes
      , Seconds
    }
}

