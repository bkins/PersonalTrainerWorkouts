using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models.ContactsAndClients
{
    [Table("PhoneNumber")]
    public class PhoneNumber
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string Number { get; set; }
        public bool   IsMain { get; set; } 
        public string Type   { get; set; } //Create a table to hold these types
        //Consider: create a Type table that holds /all/ types. Which would have a column called something like "For" that would
        //reference the table the type is /For/.
        
        [ForeignKey(typeof(Client))]
        public int ClientId { get; set; }

        public override string ToString()
        {
            return Number;
        }
    }
}