using System.Collections.Generic;
using PersonalTrainerWorkouts.Models.Intermediates;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models
{
    [Table("Equipment")]
    public class Equipment : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ManyToMany(typeof(ExerciseEquipment)
                  , CascadeOperations = CascadeOperation.All)]
        public List<Exercise> Exercises { get; set; }
    }
}
