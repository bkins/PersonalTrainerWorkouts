using PersonalTrainerWorkouts.Data.Interfaces;
using PersonalTrainerWorkouts.Models.ContactsAndClients;

using System.Collections.Generic;

using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.Data;

public class ContactsDataAccess
{
    private IContactsDataStore DataStore { get; set; }

    private AppContact _selectedAppContact;
    public AppContact SelectedContact
    {
        get => _selectedAppContact ?? SelectContact();
        set => _selectedAppContact = value;
    }

    public ContactsDataAccess(IContactsDataStore dataStore)
    {
        dataStore ??= new ContactsDataStore();
        DataStore =   dataStore;
    }

    public IEnumerable<Contact> GetDeviceContacts()
    {
        return DataStore.GetDeviceContacts();
    }

    public IEnumerable<AppContact> GetAppContacts()
    {
        return DataStore.GetAppContacts();
    }

    public void SetContacts()
    {
        DataStore.SetContacts();
    }

    public AppContact SelectContact()
    {
        return DataStore.GetSelectedAppContact();
    }

    public void UpdateClientWithContactInfo(Client client)
    {
        DataStore.UpdateClientWithContactInfo(client);
    }
}