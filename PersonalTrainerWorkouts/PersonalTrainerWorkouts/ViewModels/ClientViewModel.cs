using System;
using System.Linq;
using System.Threading.Tasks;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Services;
using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ClientViewModel : BaseViewModel
    {
        private int    Id     { get; set; }
        public  Client Client { get; set; }

        public ClientViewModel()
        {
            Client = new Client();
        }
        public ClientViewModel(string clientId)
        {
            Client = MergedClientsAndContacts.Instance
                                             .CachedClients
                                             .FirstOrDefault(client => client.ClientId.ToString() == clientId);
            // Client.PhoneNumbers.Add(new PhoneNumber
            //                         {
            //                             Type   = "Cell"
            //                           , Number = "(360) 790-8466"
            //                           , IsMain = true
            //                         });
            Save();
        }

        public async Task LinkContactToClient()
        {
            try
            {
                var contact = await Contacts.PickContactAsync();

                if(contact == null)
                    return;

                Client.Contact = contact;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Something went wrong while retrieving Contact.", Category.Error, ex);
            }
        }
        
        public void Save()
        {
            if (Client.ClientId == 0)
            {
                Id = DataAccessLayer.AddNewClient(Client);
            }
            else
            {
                DataAccessLayer.UpdateClient(Client);
            }
        }

        public int Delete()
        {
            if (Id == 0)
            {
                return 0;
            }

            return DataAccessLayer.DeleteClient(Client);
        }
    }
}