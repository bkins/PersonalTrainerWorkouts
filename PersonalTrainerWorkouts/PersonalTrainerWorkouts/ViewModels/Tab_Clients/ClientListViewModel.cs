using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avails.D_Flat.Extensions;
using Avails.Xamarin.Logger;
using Java.Lang;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using Xamarin.Essentials;
using Exception = System.Exception;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Clients;

public class ClientListViewModel : ViewModelBase
{
    public ObservableCollection<Client> ObservableClients { get; set; }

    public IList<Client> Clients  { get; set; }
    public Client        Client   { get; set; }
    public List<Contact> Contacts { get; set; }

    public ClientListViewModel()
    {
        // LoadData(DataAccessLayer.GetClients());
        LoadData();
    }

    public ObservableCollection<Client> GetObservableClients()
    {
        return ObservableClients;
    }

    // public ClientListViewModel(List<Client> aList, DataAccess dbAccessLayer)
    // {
    //     DataAccessLayer = dbAccessLayer;
    //     LoadData(aList);
    // }

    public void LoadData()
    {
        Clients = DataAccessLayer.GetClients().ToList();

        foreach (var client in Clients.Where(client => client.ContactId.HasValue()
                                                    && client.MainNumber is null))
        {
            ContactsDataAccess.UpdateClientWithContactInfo(client);

            //client.Contact = ContactsDataAccess.GetDeviceContacts().FirstOrDefault(contact => contact.Id == client.AppContactId);

            //AssignPhoneNumbersToClient(client);

            //client.Name = client.DisplayName;

            //DataAccessLayer.UpdateClient(client);
        }
        ObservableClients = new ObservableCollection<Client>(Clients); //(MergedClientsAndContacts.Instance.ClientsList);
    }

    private static void AssignPhoneNumbersToClient(Client client)
    {
        if (client.Contact is not null
         && client.Contact.Phones.Any())
        {
            foreach (var phone in client.Contact.Phones)
            {
                client.PhoneNumbers.Add(new PhoneNumber
                {
                    Number = phone.PhoneNumber
                });
            }
        }
    }

    public (string item, bool success, DeleteReasons reason) Delete(int index)
    {
        if (index > ObservableClients.Count - 1) { return (string.Empty, false, DeleteReasons.ClientNotFound); }

        //Get the client to be deleted
        var itemToDelete = ObservableClients[index];
        var name = itemToDelete.DisplayName;

        //Check if there are any Sessions for this client
        var hasSessions = DataAccessLayer.GetSessions().Any(session => session.ClientId == itemToDelete.Id);

        if (hasSessions)
        {
            return ($"There are sessions assigned to {name}", false, DeleteReasons.HasSessions);
        }

        //Remove the client from the source list
        ObservableClients.RemoveAt(index);

        var clientGoals = DataAccessLayer.GetGoals()
                                         .Where(goal => goal.ClientId == itemToDelete.Id);
        foreach (var goal in clientGoals)
        {
            DataAccessLayer.DeleteGoal(goal);
        }

        var clientMeasurables = DataAccessLayer.GetMeasurablesByClient(itemToDelete.Id);
        foreach (var measurable in clientMeasurables)
        {
            DataAccessLayer.DeleteMeasurable(measurable);
        }
        //Delete the client from the database
        var numberAffected = App.Database.DeleteClient(ref itemToDelete);
        LoadData();

        return numberAffected switch
        {
            0 => ("Client was not deleted. See Logs", false, DeleteReasons.Failure)
                  ,
            > 1 => ("More than one Client was deleted!", true, DeleteReasons.MultipleDeleted)
                  ,
            _ => (name, true, DeleteReasons.Success)
        };
    }

    public (string item, bool success, DeleteReasons reason) DeleteClientSessions(int index)
    {
        if (index > ObservableClients.Count - 1) { return (string.Empty, false, DeleteReasons.ClientNotFound); }

        var client = ObservableClients[index];
        var clientSessions = DataAccessLayer.GetSessions()
                                            .Where(session => session.ClientId == client.Id);
        var results = new StringBuilder();

        foreach (var session in clientSessions)
        {
            DataAccessLayer.DeleteSession(session);
            results.Append($"Session deleted for {client.DisplayName} on {session.Date:g}{Environment.NewLine}");
        }

        Logger.WriteLine($"Sessions deleted:{Environment.NewLine}{results.ToString()}", Category.Information);

        return ($"All sessions deleted for {client.DisplayName}.  See logs for all seesions deleted", true, DeleteReasons.Success);
    }

    public ObservableCollection<Client> SearchByClientName(string filterText)
    {
        return new ObservableCollection<Client>
                (
                 ObservableClients.Where(field => field.DisplayName
                                                       .ToUpper()
                                                       .Contains(filterText.ToUpper()))
                );
    }

    public ObservableCollection<Client> SearchClients(string filterText)
    {
        return SearchByClientName(filterText);
    }

    public async void SyncContactsToAppDatabase()
    {
        var contacts = await GetAllContactsFromPhone().ConfigureAwait(false);

        foreach (var newContact in contacts.Select(appContact => new AppContact(appContact)))
        {
            DataAccessLayer.AddNewContact(newContact);
        }
    }

    public async void SyncContacts()
    {
        var contacts = await GetAllContactsFromPhone().ConfigureAwait(false);
        var enumeratedContacts = contacts.ToList();

        foreach (var client in Clients)
        {
            var clientContactInfo = enumeratedContacts.FirstOrDefault(contact => contact.Id == client.ContactId);

            //Setting of the values from Contacts to Client.Contact is in the setter of Client.Contact
            client.Contact = clientContactInfo;

            DataAccessLayer.UpdateClient(client);
        }
    }

    private static async Task<IEnumerable<Contact>> GetAllContactsFromPhone()
    {
        var allContacts = await Xamarin.Essentials.Contacts.GetAllAsync();
        if (allContacts is null) { return new List<Contact>(); }

        var contacts = allContacts.ToList();

        return contacts;
    }

    public async Task<Client> AddNewClient()
    {
        try
        {
            var contactTask = Xamarin.Essentials.Contacts.PickContactAsync();
            var contact = await contactTask;

            //var contact = ContactsDataAccess.SelectedContact;
            //contact.Wait();

            if (contact is null) { throw new Exception("Contact is null."); }

            var newClient = new Client(contact);
            var appContact = new AppContact(contact);

            newClient.AppContact = appContact;
            newClient.Contact = contact;

            DataAccessLayer.AddNewContact(appContact);
            DataAccessLayer.AddNewClient(newClient);

            ObservableClients = new ObservableCollection<Client>(DataAccessLayer.GetClients());

            LoadData();

            return newClient;
        }
        catch (AggregateException aggregateException)
        {
            foreach (var exception in aggregateException.InnerExceptions)
            {
                Logger.WriteLine(exception.Message, Category.Error, exception);
            }
        }
        catch (Exception e)
        {
            Logger.WriteLine("Something went wrong while retrieving the contact. Probably just the user clicked back instead of selecting a contact.", Category.Error, e);
        }

        return new Client();
    }

}

public enum DeleteReasons
{
    Success
  , Failure
  , MultipleDeleted
  , HasSessions
  , ClientNotFound
}
