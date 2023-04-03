using Avails.Xamarin.Logger;

using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.AppContacts;
using PersonalTrainerWorkouts.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ClientListViewModel : BaseViewModel
    {
        public ObservableCollection<Client> ObservableClients { get; set; }
        public IList<Client> Clients { get; set; }

        public ClientListViewModel()
        {
            // LoadData(DataAccessLayer.GetClients());
            //LoadData();
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
            if (MergedClientsAndContacts.Instance.ClientsList is null) return;

            ObservableClients = new ObservableCollection<Client>(MergedClientsAndContacts.Instance.ClientsList);

            Clients = ObservableClients.ToList();
        }

        public (string item, bool success) Delete(int index)
        {
            if (index > ObservableClients.Count - 1)
            {
                return (string.Empty, false);
            }

            //Get the client to be deleted
            var itemToDelete = ObservableClients[index];
            var name = itemToDelete.DisplayName;

            //Remove the client from the source list
            ObservableClients.RemoveAt(index);

            //Delete the client from the database
            var numberAffected = App.Database.DeleteClient(ref itemToDelete);

            ObservableClients = new ObservableCollection<Client>(DataAccessLayer.GetClients());

            if (numberAffected == 0) { return ("<Client was not deleted. See Logs>", false); }
            if (numberAffected > 1) { return ("<More than one Client was deleted!", true); }

            return (name, true);
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
            var allContacts = await Contacts.GetAllAsync();
            if (allContacts is null) { return new List<Contact>(); }

            var contacts = allContacts.ToList();

            return contacts;
        }

        public async Task AddNewClient()
        {
            try
            {
                var contactTask = Contacts.PickContactAsync();
                var contact = await contactTask;

                //var contact = ContactsDataAccess.SelectedContact;
                //contact.Wait();

                if (contact is null)
                {
                    throw new Exception("Contact is null.");
                }

                var newClient = new Client(contact);

                DataAccessLayer.AddNewClient(newClient);
                _ = MergedClientsAndContacts.Instance.ForceUpdateClients(); //<-  Seems to block IO... make async

                LoadData();
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
        }
    }
}