using System;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts;
using Xamarin.Forms;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;

namespace PersonalTrainerWorkouts.Views.Tab_Workouts
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
            }
        }

        public ExerciseExistingEntryPage()
        {
            InitializeComponent();
            ItemViewModel = new ExerciseItemViewModel();
        }

        public ExerciseExistingEntryPage(string workoutId)
        {
            _workoutId = workoutId;
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
