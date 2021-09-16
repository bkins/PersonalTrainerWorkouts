using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Utilities;
using PersonalTrainerWorkouts.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(ExerciseId)
                 , nameof(ExerciseId))]
    [QueryProperty(nameof(WorkoutId)
                 , nameof(WorkoutId))]
    public partial class TypeOfExerciseEntryPage : ContentPage
    {
        private TypeOfExerciseViewModel ViewModel;

        private string _workoutId = "0";

        public string WorkoutId
        {
            get => _workoutId;
            set => LoadWorkout(value);
        }

        private string _exerciseId = "0";

        public string ExerciseId
        {
            get => _exerciseId;
            set => LoadExercise(value);
        }

        private void LoadWorkout(string workoutId)
        {
            _workoutId = workoutId;
        }

        private void LoadExercise(string exerciseId)
        {
            try
            {
                _exerciseId = exerciseId;
                ViewModel   = new TypeOfExerciseViewModel(_exerciseId);
            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed to load TypeOfExercises."
                               , Category.Error
                               , e);

                //BENDO: consider implementing a page that shows exception details
            }
        }

        public TypeOfExerciseEntryPage()
        {
            InitializeComponent();
        }

        private async void Name_OnUnfocused(object         sender
                                          , FocusEventArgs e)
        {
            var nameEntry = (Entry)sender;

            ViewModel.SaveTypeOfExercise(nameEntry.Text);

            await PageNavigation.NavigateTo(nameof(ExerciseAddEditPage)
                                          , nameof(ExerciseAddEditPage.WorkoutId)
                                          , WorkoutId
                                          , nameof(ExerciseAddEditPage.ExerciseId)
                                          , ExerciseId);
        }
    }
}
