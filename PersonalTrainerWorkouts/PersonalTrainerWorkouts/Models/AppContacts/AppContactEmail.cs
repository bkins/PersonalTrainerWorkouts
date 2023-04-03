using SQLite;

namespace PersonalTrainerWorkouts.Models.AppContacts
{
    [Table("ContactEmails")]
    public class AppContactEmail
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string EmailAddress { get; set; }

    }
}
