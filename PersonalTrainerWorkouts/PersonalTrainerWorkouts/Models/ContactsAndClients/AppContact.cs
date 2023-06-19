using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Data.Interfaces;

using SQLite;

using SQLiteNetExtensions.Attributes;

using System.Collections.Generic;
using System.Linq;

using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.Models.ContactsAndClients
{
    [Table("Contacts")]
    public class AppContact
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string ContactId { get; set; }
        public string NamePrefix { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string FamilyName { get; set; }
        public string NameSuffix { get; set; }

        public override string ToString() => DisplayName;

        private string _displayName;

        public string DisplayName
        {
            get => !string.IsNullOrWhiteSpace(_displayName) ? _displayName : BuildDisplayName();
            private set => _displayName = value;
        }

        private string BuildDisplayName()
        {
            if (string.IsNullOrWhiteSpace(GivenName))
                return FamilyName;

            return string.IsNullOrWhiteSpace(FamilyName) ?
                           GivenName :
                           $"{GivenName} {FamilyName}";
        }
    
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public new List<AppContactPhone> Phones { get; set; } = new List<AppContactPhone>();

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public new List<AppContactEmail> Emails { get; set; } = new List<AppContactEmail>();

        [ForeignKey(typeof(Client))]
        public int ClientId { get; set; }
        
        public AppContact()
        {

        }

        public AppContact(Contact contact)
        {
            ContactId = contact.Id;
            NamePrefix = contact.NamePrefix;
            GivenName = contact.GivenName;
            MiddleName = contact.MiddleName;
            FamilyName = contact.FamilyName;
            NameSuffix = contact.NameSuffix;

            var appPhones = contact.Phones
                                   .Select(contactPhone => new AppContactPhone
                                   {

                                       PhoneNumber = contactPhone.PhoneNumber

                                   })
                                   .ToList();

            Phones.AddRange(appPhones);

            var appEmails = contact.Emails.Select(email => new AppContactEmail
            {
                EmailAddress = email.EmailAddress
            }).ToList();

            Emails.AddRange(appEmails);
        }

        public Contact ToContact(IContactsDataStore contactsDataStore, AppContact selectedContact = null)
        {
            if (contactsDataStore is null) return new Contact();
            
            var contactsDataAccess = new ContactsDataAccess(contactsDataStore);

            if (selectedContact is not null)
            {
                contactsDataAccess.SelectedContact = selectedContact;
            }
            return contactsDataAccess.GetDeviceContacts().FirstOrDefault(contact => contact.Id == ContactId);
        }
    }
}
