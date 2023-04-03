using SQLite;

namespace PersonalTrainerWorkouts.Models
{
    internal class UnitOfMeasurement
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int Unit { get; set; }
    }
}
