using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Views.Converters
{
        public class HeightConverter<T> : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return parameter is SfListView listView 
                    && value is ObservableCollection<T> items ?
                               items.Count * listView.ItemSize :
                               0;
            }

            public object ConvertBack(object      value
                                    , Type        targetType
                                    , object      parameter
                                    , CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
}
