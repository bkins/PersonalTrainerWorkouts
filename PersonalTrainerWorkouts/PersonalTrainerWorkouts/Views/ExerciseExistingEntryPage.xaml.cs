using System;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using Xamarin.Forms;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    [QueryProperty(nameof(WorkoutId)
                 , nameof(WorkoutId))]
    public partial class ExerciseExistingEntryPage
    {
        private ExerciseItemViewModel ItemViewModel { get; set; }

        private string _workoutId = "0";

        public string WorkoutId
        {
            get => _workoutId;
            set => LoadExercises(value);
        }

        private void LoadExercises(string workoutId)
        {
            try
            {
                _workoutId = workoutId;

                ExercisePicker.ItemsSource = ItemViewModel.AllExercises;
            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed to load Exercises."
                               , Category.Error
                               , e);

                //BENDO: consider implementing a page that shows exception details
            }
        }

        public ExerciseExistingEntryPage()
        {
            InitializeComponent();
            ItemViewModel = new ExerciseItemViewModel();
        }

        private void ExercisePicker_OnOkButtonClicked(object                    sender
                                                          , SelectionChangedEventArgs e)
        {
            ItemViewModel.SelectedExercise = (Exercise)ExercisePicker.SelectedItem;

            ItemViewModel.SaveExercise(int.Parse(WorkoutId));

            PageNavigation.NavigateBackwards();
        }

        private void ExercisePicker_OnCancelButtonClicked(object                    sender
                                                              , SelectionChangedEventArgs e)
        {
            PageNavigation.NavigateBackwards();
        }
    }
}
