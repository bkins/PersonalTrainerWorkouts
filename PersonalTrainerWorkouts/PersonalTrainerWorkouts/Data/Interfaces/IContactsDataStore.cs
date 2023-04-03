using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.Data.Interfaces;

public interface IContactsDataStore
{
    IEnumerable<Contact> GetContacts();
    void SetContacts();
    Task<Contact> SelectContact();
}