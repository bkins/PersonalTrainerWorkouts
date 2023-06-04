using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PersonalTrainerWorkouts.Data;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected static DataAccess _dataAccess;

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
            set => SetValue(ref _isBusy, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this
                                  , new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T                     backingField
                                 , T                         value
                                 , [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField
                                                 , value))
            {
                return;
            }

            backingField = value;

            OnPropertyChanged(propertyName);
        }
    }
}
