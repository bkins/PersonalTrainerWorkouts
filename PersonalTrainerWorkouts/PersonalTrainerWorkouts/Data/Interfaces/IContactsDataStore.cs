using PersonalTrainerWorkouts.Models.ContactsAndClients;

using System.Collections.Generic;

using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.Data.Interfaces;

public interface IContactsDataStore
{
    IEnumerable<AppContact> GetAppContacts();
    IEnumerable<Contact> GetDeviceContacts();

    void SetContacts();

    AppContact GetSelectedAppContact();

    void UpdateClientWithContactInfo(Client client);
}