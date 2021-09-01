using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TypeOfExerciseEntryPage : ContentPage
    {
        public TypeOfExerciseEntryPage()
        {
            InitializeComponent();
        }

        private void TypeOfExercisePicker_OnOkButtonClicked(object                    sender
                                                          , SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}