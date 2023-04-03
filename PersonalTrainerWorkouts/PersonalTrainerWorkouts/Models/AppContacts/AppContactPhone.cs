using SQLite;

namespace PersonalTrainerWorkouts.Models.AppContacts
{
    [Table("ContactPhones")]
    public class AppContactPhone
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string PhoneNumber { get; set; }

    }
}
