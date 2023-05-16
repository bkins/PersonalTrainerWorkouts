using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models.ContactsAndClients
{
    [Table("ContactEmails")]
    public class AppContactEmail
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string EmailAddress { get; set; }

        [ForeignKey(typeof(AppContact))]
        public int AppContactId { get; set; }
    }
}
