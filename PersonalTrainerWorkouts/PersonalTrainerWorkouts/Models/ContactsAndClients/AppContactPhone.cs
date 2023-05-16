using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models.ContactsAndClients
{
    [Table("ContactPhones")]
    public class AppContactPhone
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string PhoneNumber { get; set; }

        [ForeignKey(typeof(AppContact))]
        public int AppContactId { get; set; }
    }
}
