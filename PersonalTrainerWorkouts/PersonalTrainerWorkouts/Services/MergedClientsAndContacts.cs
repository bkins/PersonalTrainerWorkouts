using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        get => _instance ??= new();
    }

    private IEnumerable<Contact> CachedContacts { get; set; }
    private List<Contact> _contacts;

    public IEnumerable<Client> CachedClients { get; set; }
    public List<Client> ClientsList { get; set; }

    private DataAccess _dataAccess;
    public DataAccess DataAccessLayer
    {
        get => _dataAccess ??= new(App.Database);
        set => _dataAccess = value;
    }

    private ContactsDataAccess _contactsDataAccess;

    public ContactsDataAccess ContactsDataAccessLayer
    {
        get => _contactsDataAccess ??= new(App.ContactDataStore);
        set => _contactsDataAccess = value;
    }

    public MergedClientsAndContacts()
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
        var contact = _contacts.FirstOrDefault(contact => contact.Id
                                                       == client.ContactId);
        if (contact is null)
        {
            ForceUpdateContacts();
        }
        else
        {
            return contact;
        }

        contact = _contacts.FirstOrDefault(cachedContact => cachedContact.Id
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
            || !client.PhoneNumbers.Any()
            || client.PhoneNumbers.Any(numbers => numbers.IsMain);
    }

    public void ForceUpdateContacts()
    {
        CachedContacts = ContactsDataAccessLayer.GetContacts();
        _contacts = CachedContacts.ToList();

        UpdateAll(forceRefresh: false);
    }

    public async Task ForceUpdateClients()
    {
        CachedClients = await Task.Run(() => DataAccessLayer.GetClients());
        ClientsList = CachedClients.ToList();

        if (ClientsList is null) return;

        foreach (var client in ClientsList)
        {
            var contact = GetContact(client);

            client.Contact = contact;

            var hasAnyIsMain = client.PhoneNumbers.Any(phone => phone.IsMain);

            DefaultPhoneNumbersIsMain(client);

            hasAnyIsMain = client.PhoneNumbers.Any(phone => phone.IsMain);
        }
        //UpdateAll(forceRefresh: false);
    }
}