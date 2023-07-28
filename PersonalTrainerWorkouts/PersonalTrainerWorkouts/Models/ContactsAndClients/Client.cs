using Avails.D_Flat.Extensions;
using Avails.Xamarin.Logger;
using SQLite;

using SQLiteNetExtensions.Attributes;

using System.Collections.Generic;
using System.Linq;
using PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;
using Xamarin.Essentials;
using Xamarin.Forms;
using Exception = Java.Lang.Exception;

namespace PersonalTrainerWorkouts.Models.ContactsAndClients
{
    [Table($"{nameof(Client)}s")]
    public class Client : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(AppContact))]
        public int AppContactId { get; set; }

        public string ContactId { get; set; }
        public string Notes { get; set; }
        public string MainNumber { get; set; } //Since there is no flag from Contacts that specifies the main number
                                               //just store the "Main" number as-is. This field to identify which number
                                               //in the Contact to use as the main number.

        public string BackgroundColorHex  { get; set; }
        public string TextColorHex { get; set; }

        [Ignore]
        public Color BackgroundColor => GetColorFromHex();

        [Ignore]
        public Color TextColor => GetTextColorFromHex();

        private Color GetTextColorFromHex()
        {
            if (TextColorHex.IsNullEmptyOrWhitespace()) TextColorHex = Color.Black.ToHex();

            try
            {
                return Color.FromHex(TextColorHex);
            }
            catch (Exception e) { /* swallow error and use default value below*/ }

            return Color.Black;
        }

        private Color GetColorFromHex()
        {
            if (BackgroundColorHex.IsNullEmptyOrWhitespace()) BackgroundColorHex = Color.White.ToHex();

            try
            {
                return Color.FromHex(BackgroundColorHex);
            }
            catch (Exception e) { /* swallow error and use default value below*/ }

            return Color.White;
        }
        private Contact _contact { get; set; }

        [Ignore]
        public Contact Contact
        {
            get => _contact;
            set
            {
                if (value is null)
                {
                    Logger.WriteLine($"For the Client with the Id of {Id}, an attempt to assign the Contact with a null value. This will probably resolve its self the next time the cached clients and contacts are updated.", Category.Warning);
                    return;
                }

                _contact = value;

                AddPhoneNumbersToClient();
                //TODO: Finish: AddEmailsToClient();

                ContactId = _contact.Id;
            }
        }

        private void AddEmailsToClient()
        {
            //TODO: Add emails to this (Client) class
        }

        private void AddPhoneNumbersToClient()
        {
            var contactPhones = _contact.Phones.Where(phone => PhoneNumbers.All(numbers => numbers.Number != phone.PhoneNumber));

            foreach (var phone in contactPhones)
            {
                PhoneNumbers.Add(new PhoneNumber
                {
                    Number = phone.PhoneNumber
                });
            }
        }

        public void SetName()
        {
            Name = DisplayName;
        }

        public string SetMainNumber()
        {
            if (!PhoneNumbers.Any())
            {
                return string.Empty;
            }

            if (MainNumber.HasValue()
             && PhoneNumbers.Any(number => number.Number == MainNumber))
            {
                PhoneNumbers.FirstOrDefault(number => number.Number == MainNumber)
                           !.IsMain = true;
            }

            if (PhoneNumbers.All(number => !number.IsMain)
             || PhoneNumbers.Count == 1)
            {
                //If only one number it is by default the main number
                //Or if there are none that are flagged as Main, then use the first one
                MainNumber = PhoneNumbers.FirstOrDefault()
                                        ?.Number;
            }
            else if (PhoneNumbers.Count > 1)
            {
                //There are multiple numbers and one of them is Main
                MainNumber = PhoneNumbers.FirstOrDefault(fields => fields.IsMain)
                                        ?.Number;
            }

            return MainNumber;
        }

        private string _displayName;

        [Ignore]
        public string DisplayName
        {
            get => _displayName.HasValue()
                           ? _displayName
                           : BuildDisplayName();

            private set => _displayName = value;
        }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public AppContact AppContact { get; set; } = new();

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PhoneNumber> PhoneNumbers { get; set; } = new();

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Address> Addresses { get; set; } = new();

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Measurable> Measurements { get; set; } = new();

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Measurable> Maxes { get; set; } = new();

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Goal> Goals { get; set; } = new();

        public Client()
        {
            BackgroundColorHex = BackgroundColor.ToHex();
            TextColorHex       = TextColor.ToHex();
        }

        public Client(Contact contact) : this()
        {
            Contact = contact;
        }

        private string BuildDisplayName()
        {
            return Contact is null ? GetDisplayNameFromContact() : GetDisplayNameFromAppContact();
        }

        private string GetDisplayNameFromContact()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Contact?.GivenName))
                    return Contact?.FamilyName;

                if (string.IsNullOrWhiteSpace(Contact?.FamilyName))
                    return Contact?.GivenName;
            }
            catch
            {
                return string.Empty;
            }

            return $"{Contact.GivenName} {Contact.FamilyName}";
        }
        private string GetDisplayNameFromAppContact()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AppContact?.GivenName))
                    return AppContact?.FamilyName;

                if (string.IsNullOrWhiteSpace(AppContact?.FamilyName))
                    return AppContact?.GivenName;
            }
            catch
            {
                return string.Empty;
            }

            return $"{AppContact.GivenName} {AppContact.FamilyName}";
        }
        //public override string ToString() => DisplayName ?? $"No {nameof(DisplayName)} set";
        public override string ToString()
        {
            // var data = new StringBuilder();
            //
            // data.Append($"\n\tName:        {Name}");
            // data.Append($"\n\tDisplayName: {DisplayName}");
            // data.Append($"\n\tMainNumber:  {MainNumber};");
            // data.Append($"\n\tId:          {Id}");
            //
            // return data.ToString();
            return DisplayName;
        }

    }
}
