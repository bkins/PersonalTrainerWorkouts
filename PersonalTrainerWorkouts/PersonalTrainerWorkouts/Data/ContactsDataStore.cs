using PersonalTrainerWorkouts.Data.Interfaces;
using PersonalTrainerWorkouts.Models.ContactsAndClients;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.Data;

public class ContactsDataStore : IContactsDataStore
{
    public IEnumerable<AppContact> AppContacts { get; private set; }
    public IEnumerable<Contact> DeviceContacts { get; set; }

    private DataAccess _dataAccess;

    public DataAccess DataAccessLayer
    {
        get => _dataAccess = _dataAccess ?? new DataAccess(App.Database, App.ContactDataStore);
        set => _dataAccess = value;
    }


    public AppContact SelectedContact
    {
        get => GetSelectedAppContact();
    }

    public ContactsDataStore()
    {
        SetContacts();
    }

    public ContactsDataStore(IEnumerable<Contact> mockContacts)
    {
        DeviceContacts = mockContacts;
    }

    public IEnumerable<AppContact> GetAppContacts()
    {
        return AppContacts;
    }

    public IEnumerable<Contact> GetDeviceContacts()
    {
        return DeviceContacts;
    }

    /// <summary>
    /// Sets App Contacts and Device Contacts
    /// </summary>
    public async void SetContacts()
    {
        var contacts = await Contacts.GetAllAsync().ConfigureAwait(false);

        DeviceContacts = contacts.ToList();
    }

    public void UpdateClientWithContactInfo(Client client)
    {
        client.Contact = GetDeviceContacts().FirstOrDefault(contact => contact.Id == client.ContactId);
        //TODO: Phone number may already be assign
        UpdateClientPhoneNumbersFromContact(client);

        client.Name = client.DisplayName;
        client.SetMainNumber();

        DataAccessLayer.UpdateClient(client);
    }

    private static void UpdateClientPhoneNumbersFromContact(Client client)
    {
        if (client.Contact is null
         || ! client.Contact.Phones.Any())
            return;

        foreach (var phone in client.Contact
                                    .Phones
                                    .Where(phone => client.PhoneNumbers
                                                          .All(number => number.Number != phone.PhoneNumber)))
        {
            client.PhoneNumbers.Add(new PhoneNumber
                                    {
                                        Number = phone.PhoneNumber
                                    });
        }
    }

    public AppContact GetSelectedAppContact()
    {
        var contactFromDevice = SelectContactFromDevice();
        var selectedContactAsAppContact = new AppContact(contactFromDevice.Result);

        return selectedContactAsAppContact;
    }

    public Contact GetSelectedDeviceContact()
    {
        var contact = SelectContactFromDevice();
        contact.Wait(millisecondsTimeout: 10000);

        return contact.Result;
    }

    private async Task<Contact> SelectContactFromDevice()
    {
        return await Contacts.PickContactAsync();
    }

    private static IEnumerable<AppContact> ToAppContacts(IEnumerable<Contact> contacts)
    {
        return contacts.Select(thisContact => new AppContact(thisContact))
                       .ToList();
    }
}