using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;
using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Clients
{
    public class ClientViewModel : ViewModelBase
    {
        public  Client                     Client                { get; set; }
        public  List<GoalViewModel>        Goals                 { get; set; }
        public  List<MeasurablesViewModel> Measurables           { get; set; }
        public  IEnumerable<PhoneNumber>   PhoneNumbers          { get; set; }
        public  bool                       ShowChangeNumberImage { get; }

        //public List<GoalViewModel> Goals { get; set; }

        public ClientViewModel()
        {
            Client = new Client();
        }

        public ClientViewModel(string clientId)
        {
            Client = DataAccessLayer.GetClients()
                                    .FirstOrDefault(client => client.Id.ToString() == clientId);
            PhoneNumbers = Client?.PhoneNumbers;
            
            ContactsDataAccess.UpdateClientWithContactInfo(Client);

            ShowChangeNumberImage = Client?.PhoneNumbers.Count > 1;

            SetListOfGoalViewModels();
            SetListOfMeasurableViewModels();

            // PhoneNumbers = new List<PhoneNumber>();
            // var clientNumbers = Client?.PhoneNumbers;
            //
            // if (clientNumbers is null) return;
            //
            // foreach (var number in clientNumbers)
            // {
            //     PhoneNumbers.Add(number);
            // }
        }

        private void SetListOfMeasurableViewModels()
        {
            Measurables = new List<MeasurablesViewModel>();

            var measurables    = DataAccessLayer.GetMeasurables()
                                                .Where(measurable => measurable.ClientId == Client.Id)
                                                .ToList();
            foreach (var measurable in measurables)
            {
                if (measurable.Type == null) continue;
                var thisMeasurable = new MeasurablesViewModel
                                     {
                                         Variable          = measurable.Variable
                                       , Value             = measurable.Value
                                       , DateTaken         = measurable.DateTaken
                                       , Id                = measurable.Id
                                       , Type              = measurable.Type
                                       , GoalSuccession    = measurable.GoalSuccession
                                       , UnitOfMeasurement = measurable.UnitOfMeasurement
                                     };
                thisMeasurable.NewMeasurable ??= new Measurable();

                Measurables.Add(thisMeasurable);

            }
        }

        private void SetListOfGoalViewModels()
        {
            Goals = new List<GoalViewModel>();
            var clientGoals = Client?.Goals;

            if (clientGoals == null) return;

            foreach (var clientGoal in clientGoals)
            {
                Goals.Add(new GoalViewModel(clientGoal));
            }
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
            return Client.Id == 0
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
