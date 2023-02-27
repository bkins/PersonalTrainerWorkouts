﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PersonalTrainerWorkouts.Data.Interfaces;
using Xamarin.Essentials;

namespace PersonalTrainerWorkouts.Data;

public class ContactsDataStore : IContactsDataStore
{
    public  IEnumerable<Contact> Contacts { get; private set; }

    public ContactsDataStore ()
    {
        SetContacts();
    }

    public IEnumerable<Contact> GetContacts()
    {
        return Contacts;
    }
    
    public async void SetContacts()
    {
        Contacts = await Xamarin.Essentials.Contacts.GetAllAsync().ConfigureAwait(false);
    }
}