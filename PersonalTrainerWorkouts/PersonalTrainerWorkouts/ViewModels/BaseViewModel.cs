using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PersonalTrainerWorkouts.Data;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class BaseViewModel
    {
        private DataAccess _dataAccess;
    
        public DataAccess DataAccessLayer
        {
            get => _dataAccess = _dataAccess ?? new DataAccess(App.Database, App.ContactDataStore);
            set => _dataAccess = value;
        }

        private ContactsDataAccess _contactsDataAccess;

        public ContactsDataAccess ContactsDataAccess
        {
            get => _contactsDataAccess = _contactsDataAccess ?? new ContactsDataAccess(App.ContactDataStore);
            set => _contactsDataAccess = value;
        }
        
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        protected bool SetProperty<T>(ref T                     backingStore
                                    , T                         value
                                    , [CallerMemberName] string propertyName = ""
                                    , Action                    onChanged    = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;

            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
