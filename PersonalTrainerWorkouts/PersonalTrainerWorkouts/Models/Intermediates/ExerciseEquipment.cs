using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models.Intermediates
{
    [Table("ExerciseEquipment")]
    public class ExerciseEquipment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Exercise))]
        public int ExerciseId  { get; set; }

        [ForeignKey(typeof(Equipment))]
        public int EquipmentId { get; set; }
    }
}
