using System.Collections.Generic;
using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.Data.Interfaces;

public interface IContactsDataStore
{
    IEnumerable<Contact> GetContacts();
    void SetContacts();
}