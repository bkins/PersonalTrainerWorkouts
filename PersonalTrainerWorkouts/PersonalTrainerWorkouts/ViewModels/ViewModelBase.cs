using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PersonalTrainerWorkouts.Data;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private static DataAccess _dataAccess;

        public DataAccess DataAccessLayer
        {
            get => _dataAccess = _dataAccess ?? new DataAccess(App.Database);
            set => _dataAccess = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this
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
