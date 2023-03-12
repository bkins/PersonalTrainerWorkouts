using System.Collections.Generic;
using System.Linq;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.Services;

public sealed class MergedClientsAndContacts
{
    private static MergedClientsAndContacts _instance;
    public static MergedClientsAndContacts Instance
    {
        get => _instance ??= new ();
    }
    
    private IEnumerable<Contact> CachedContacts { get; set; }
    public IEnumerable<Client>  CachedClients  { get; set; }

    private DataAccess _dataAccess;
    public DataAccess DataAccessLayer
    {
        get => _dataAccess ??= new (App.Database);
        set => _dataAccess = value;
    }

    private ContactsDataAccess _contactsDataAccess;

    public ContactsDataAccess ContactsDataAccessLayer
    {
        get => _contactsDataAccess ??= new(App.ContactDataStore);
        set => _contactsDataAccess = value;
    }
    
    public MergedClientsAndContacts ()
    {
        UpdateAll(forceRefresh: true);
    }

    /// <summary>
    /// Reassigns Contacts to Clients. If you set forceRefresh to true it will
    /// Re-get all Contracts from the phone and all Clients from the database first.
    /// If you set forceRefresh to false, call either ForceUpdateContacts() or ForceUpdateClients().
    /// Otherwise, it will just reassign the Contracts that already exist with the current Clients.
    /// </summary>
    /// <param name="forceRefresh">Re-get all Contracts from the phone and all Clients
    /// from the database before reassigning Contracts to Clients.</param>
    public void UpdateAll(bool forceRefresh)
    {
        if (forceRefresh)
        {
            ForceUpdateContacts();
            ForceUpdateClients();
        }

        if (CachedClients is null) return;
        
        foreach (var client in CachedClients)
        {
            var contact = GetContact(client);

            client.Contact = contact;

            DefaultPhoneNumbersIsMain(client);
        }
    }

    private Contact GetContact(Client client)
    {
        var contact = CachedContacts.FirstOrDefault(contact => contact.Id
                                                            == client.ContactId);
        if (contact is null)
        {
            ForceUpdateContacts();
        }
        else
        {
            return contact;
        }

        contact = CachedContacts.FirstOrDefault(cachedContact => cachedContact.Id
                                                              == client.ContactId);
        if (contact is null)
        {
            Logger.WriteLine($"A Client (ClientId = {client.ClientId}) has a ContactId ({client.ContactId}) that cannot be found. Was a Contact removed from the phone?", Category.Warning);
        }
        
        return contact;
    }

    private void DefaultPhoneNumbersIsMain(Client client)
    {
        if (NoNeedToUpdateIsMain(client))
            return;

        client.PhoneNumbers
              .FirstOrDefault()
              !.IsMain = true;
            
        DataAccessLayer.UpdateClient(client);
    }

    private static bool NoNeedToUpdateIsMain(Client client)
    {
        return client is null 
            || ! client.PhoneNumbers.Any()
            || client.PhoneNumbers.Any(numbers => numbers.IsMain);
    }

    public void ForceUpdateContacts()
    {
        CachedContacts = ContactsDataAccessLayer.GetContacts();
        UpdateAll(forceRefresh: false);
    }

    public void ForceUpdateClients()
    {
        CachedClients = DataAccessLayer.GetClients();
        
        if (CachedClients is null) return;
        
        foreach (var client in CachedClients)
        {
            var contact = GetContact(client);

            client.Contact = contact;

            DefaultPhoneNumbersIsMain(client);
        }
        //UpdateAll(forceRefresh: false);
    }
}