using System.Collections.Generic;
using System.Linq;
using Avails.Xamarin.Logger;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.Models
{
    [Table(nameof(Client))]
    public class Client : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int ClientId { get; set; }
        
        public  string  ContactId { get; set; }
        
        private Contact _contact { get; set; }
        [Ignore]
        public  Contact Contact   
        {
            get => _contact;
            set
            {
                if (value is null)
                {
                    Logger.WriteLine($"For the Client with the Id of {ClientId}, an attempt to assign the Contact with a null value. This will probably resolve its self the next time the cached clients and contacts are updated.", Category.Warning);
                    return;
                }
                
                _contact = value;

                var contactPhones = _contact.Phones
                                            .Where(phone => PhoneNumbers.All(numbers => numbers.Number 
                                                                                     != phone.PhoneNumber));
                
                foreach (var phone in contactPhones)
                {
                    PhoneNumbers.Add(new PhoneNumber{Number = phone.PhoneNumber} );
                }

                ContactId = _contact.Id;
            }
        }
        
        private string  _displayName;
        [Ignore]
        public string DisplayName
        {
            get => !string.IsNullOrWhiteSpace(_displayName) ? _displayName : BuildDisplayName();
            private set => _displayName = value;
        }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PhoneNumber> PhoneNumbers { get; set; } = new ();

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Address> Addresses { get; set; } = new ();

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Measurable> Measurements { get; set; } = new ();

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Measurable> Maxes { get; set; } = new ();

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Goal> Goals { get; set; } = new ();
        
        [Ignore]
        public string MainNumber 
        {
            get
            {
                string mainNumber;
                
                if (PhoneNumbers.Count == 1)
                {
                    //If only one number it is by default the main number
                    mainNumber = PhoneNumbers.FirstOrDefault()
                                             ?.Number;
                }
                else
                {
                    mainNumber = PhoneNumbers.FirstOrDefault(fields => fields.IsMain)
                                             ?.Number;    
                }
                
                return   mainNumber;
            }
        }

        public Client ()
        {
                
        }

        public Client (Contact contact) : this()
        {
            Contact = contact;
        }
        
        private string BuildDisplayName()
        {
            if (string.IsNullOrWhiteSpace(Contact.GivenName))
                return Contact.FamilyName;
            if (string.IsNullOrWhiteSpace(Contact.FamilyName))
                return Contact.GivenName;

            return $"{Contact.GivenName} {Contact.FamilyName}";
        }
        
        public override string ToString() => DisplayName;
    }
}