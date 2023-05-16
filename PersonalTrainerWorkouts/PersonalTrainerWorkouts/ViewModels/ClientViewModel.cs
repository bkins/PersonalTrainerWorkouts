using Avails.Xamarin.Logger;

using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.ContactsAndClients;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ClientViewModel : BaseViewModel
    {
        private int Id { get; set; }
        public Client Client { get; set; }

        public bool ShowChangeNumberImage { get ; }

        //public List<GoalViewModel> Goals { get; set; }

        public ClientViewModel()
        {
            Client = new Client();
        }
        public ClientViewModel(string clientId)
        {
            Client = DataAccessLayer.GetClients()
                                    .FirstOrDefault(client => client.Id.ToString() == clientId);

            ContactsDataAccess.UpdateClientWithContactInfo(Client);

            ShowChangeNumberImage = Client?.PhoneNumbers.Count > 1;

            //Goals = new List<GoalViewModel>();
            //var clientGoals = Client?.Goals;

            //if (clientGoals == null) return;

            //foreach (var clientGoal in clientGoals)
            //{
            //    Goals.Add(new GoalViewModel(clientGoal));
            //}
        }

        /// <summary>
        /// Opens Contacts on device for user to choose a Contact to link to the Client
        /// </summary>
        public async Task LinkContactToClient()
        {
            try
            {
                var contact = await Contacts.PickContactAsync();

                if (contact == null)
                    return;

                Client.AppContact = new AppContact(contact);
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Something went wrong while retrieving Contact.", Category.Error, ex);
            }
        }

        public void Save()
        {
            if (Client.Id == 0)
            {//Add new

                Client.Contact = new Contact();
                // DataAccessLayer.AddNewClientWithChildren(Client);
                
                DataAccessLayer.AddNewClient(Client);
                
                foreach (var phoneNumber in Client.PhoneNumbers)
                {
                    phoneNumber.ClientId = Client.Id;
                    DataAccessLayer.AddNewPhone(phoneNumber);
                }

                Client.AppContact.ClientId = Client.Id;
                
                DataAccessLayer.AddNewContact(Client.AppContact);
                
                var newClient = DataAccessLayer.GetClients()
                                               .First(client => client.Id == Client.Id);

                Id = newClient.Id;
            }
            else
            {//Update existing
                DataAccessLayer.UpdateClient(Client);
            }
        }

        public void Save(Goal goal)
        {
            if (goal.Id == 0)
            {
                DataAccessLayer.AddNewGoal(goal);
                Client.Goals.Add(goal);
                Save();

                return;
            }

            DataAccessLayer.UpdateGoal(goal);
        }

        public int Delete()
        {
            return Id == 0
                    ? 0
                    : DataAccessLayer.DeleteClient(Client);
        }

        public void SetNewMainNumber(PhoneNumber newMainNumber)
        {
            var oldMainNumber = Client.PhoneNumbers.FirstOrDefault(number => number.IsMain);

            if (oldMainNumber is not null) { oldMainNumber.IsMain = false; }

            var mainNumberToUpdate = Client.PhoneNumbers.FirstOrDefault(number => number.Number == newMainNumber.Number);

            if (mainNumberToUpdate is not null)
            {
                newMainNumber.IsMain = true;
                Client.MainNumber    = newMainNumber.Number;
            }
            
            Save();
        }
    }
}