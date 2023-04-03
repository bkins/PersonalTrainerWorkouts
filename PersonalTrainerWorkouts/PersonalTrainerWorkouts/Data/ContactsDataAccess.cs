using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalTrainerWorkouts.Data.Interfaces;
using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.Data;

public class ContactsDataAccess
{
    private IContactsDataStore DataStore       { get; set; }

    public Task<Contact> SelectedContact
    {
        get => SelectContact();
    }

    public ContactsDataAccess (IContactsDataStore dataStore)
    {
        DataStore = dataStore;
    }

    public IEnumerable<Contact> GetContacts()
    {
        return DataStore.GetContacts();
    }
    
    public void SetContacts()
    {
        DataStore.SetContacts();
    }

    public async Task<Contact> SelectContact()
    {
        return await DataStore.SelectContact();
    }
}