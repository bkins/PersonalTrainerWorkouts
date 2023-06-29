using SQLite;

namespace PersonalTrainerWorkouts.Models.ContactsAndClients.Goals
{
    [Table($"{nameof(UnitOfMeasurement)}s")]
    public class UnitOfMeasurement
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Unit { get; set; }

        public override string ToString()
        {
            return Unit;
        }
    }
}
