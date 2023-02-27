﻿using System.Collections.Generic;
using PersonalTrainerWorkouts.Data.Interfaces;
using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.Data;

public class ContactsDataAccess
{
    private IContactsDataStore DataStore { get; set; }

    public ContactsDataAccess (IContactsDataStore dataStore)
    {
        DataStore = dataStore;
    }

    public IEnumerable<Contact> GetContacts()
    {
        return DataStore.GetContacts();
    }
    public void SetContacts()
    {
        DataStore.SetContacts();
    }
}