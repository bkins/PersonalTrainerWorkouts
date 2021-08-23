using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Utilities;
using PersonalTrainerWorkouts.ViewModels;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class ExerciseExistingEntryPage : ContentPage, INotifyPropertyChanged
    {
        private bool              _loading = true;
        private ExerciseItemViewModel ItemViewModel { get; set; }

        private string            _itemId = "0";

        public string ItemId
        {
            get => _itemId;
            set => LoadExercises(value);
        }
        public List<Exercise> AllExercisesList { get; set; }
        //public List<Exercises> AllExercises { get; set; }  //=> App.Database.GetAllExercisesAsync().Result;
        public Exercise[] AllExercises { get; set; }

        private Exercise _selectedExercise;

        public Exercise SelectedExercise
        {
            get => _selectedExercise;
            set
            {
                if (_selectedExercise != value)
                {
                    _selectedExercise = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private async void LoadExercises(string itemId)
        {
            try
            {
                _itemId = itemId;
                var id      = Convert.ToInt32(itemId);
                //AllExercises = await App.AsyncDatabase.GetAllExercisesAsync();
                AllExercises = App.Database.GetExercises() as Exercise[];
                //var listOfExercises  = await App.AsyncDatabase.GetObservableCollectionOfExercisesAsync();
                var listOfExercises = AllExercises;

                ExercisePicker.ItemsSource   = ItemViewModel.AllExercises;
                //ExercisePicker.SelectedIndex = -1;


                //AllExercisesList           = listOfExercises;
                //ExercisePicker.ItemsSource = AllExercisesList;
                //ExercisePicker = new ExistingExerciseList
                //                 {
                //                     ColumnHeaderTextCollection = new ObservableCollection<string>
                //                                                  {
                //                                                      "Name"
                //                                                    , "LengthOfTime"
                //                                                  }

                //                   , ExercisesList       = listOfExercises
                //                   , HeaderText          = "Exercise Picker Header"
                //                   , IsEnableAutoReverse = false
                //                   , IsEnableBorderColor = true
                //                   , IsShowColumnHeader  = true
                //                   , IsShowHeader        = true
                //                   , Name                = "PickerName"
                //                   , SelectedIndex       = 0
                //                   , ItemsSource         = listOfExercises
                //                 };

                //ExercisePicker.ItemsSource = new ExistingExerciseList()
                //{
                //    ColumnHeaderTextCollection = new List<string>
                //                                 {
                //                                     "Name"
                //                                   , "LengthOfTime"
                //                                 }
                //                               , ExercisesList       = listOfExercises
                //                               , HeaderText          = "Exercise Picker Header"
                //                               , IsEnableAutoReverse = false
                //                               , IsEnableBorderColor = true
                //                               , IsShowColumnHeader  = true
                //                               , IsShowHeader        = true
                //                               , Name                = "PickerName"
                //                               , SelectedIndex       = 0

                //};

            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed to load Workout.", Category.Error, e);
                //BENDO: consider implementing a page that shows exception details
            }

            _loading = false;
        }

        public ExerciseExistingEntryPage()
        {
            InitializeComponent();
            ItemViewModel = new ExerciseItemViewModel();

            // BindingContext = new Exercises();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            //AllExercises = await App.Database.GetAllExercisesAsync();
            //SetAllExercises();
            //BindingContext = App.Database.GetAllExercisesAsync()
            //    .Result;
        }

        private async void SetAllExercises()
        {
            //AllExercises = await App.AsyncDatabase.GetAllExercisesAsync();
            AllExercises = App.Database.GetExercises() as Exercise[];
        }

        async void OnSaveButtonClicked(object    sender,
                                       EventArgs e)
        {
            var exercise = (Exercise)BindingContext;
            if ( ! string.IsNullOrWhiteSpace(exercise.Name))
            {
                //var workout  = App.AsyncDatabase.GetWorkoutsAsync(Convert.ToInt32(ItemId)).Result;
                var workout = App.Database.GetWorkout(Convert.ToInt32(ItemId));

                workout.Exercises.Add(exercise);
                //await App.AsyncDatabase.SaveWorkoutAsync(workout);
                App.Database.UpdateWorkout(workout);
                
                await PageNavigation.NavigateTo(nameof(ExerciseListPage), nameof(ExerciseListPage.WorkoutId), workout.Id.ToString());
                //// Navigate backwards
                //await Shell.Current.GoToAsync("..");
            }

        }

        private void OnDeleteButtonClicked(object    sender,
                                           EventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void ExercisePicker_OnSelectionChanged(object                    sender
                                                           , SelectionChangedEventArgs e)
        {
            return;

            if (_loading 
             || ExercisePicker.SelectedIndex == null 
             || ((int)ExercisePicker.SelectedIndex) == -1)
                return;

        }

        private async void ExercisePicker_OnOkButtonClicked(object                    sender
                                                          , SelectionChangedEventArgs e)
        {
            ItemViewModel.SelectedExercise = (Exercise) ExercisePicker.SelectedItem;

            ItemViewModel.SaveExercise(Convert.ToInt32(ItemId));
            
            //if (ViewModel.SelectedExercise == null)
            //{
            //    return;
            //}
            ////var workout  = App.AsyncDatabase.GetWorkoutsAsync(Convert.ToInt32(ItemId)).Result;
            //var workout = App.Database.GetWorkout(Convert.ToInt32(ItemId));
            //workout.Exercises.Add(exercise);
            
            ////await App.AsyncDatabase.SaveNewWorkoutExerciseAsync(workout.Id, exercise.Id, exercise.LengthOfTime);
            //App.Database.UpdateWorkout(workout);

            ////if ( ! workout.Exercises.Contains(exercise))
            ////{
            ////    await App.Database.SaveExerciseAsync(exercise);
            ////    workout.Exercises.Add(exercise);
            ////}
            
            //////BUG:  When adding an existing exercise to a workout it is only saved when you add a new exercise

            ////await App.Database.SaveWorkoutAsync(workout);

            //////App.Database.FillModels();

            ////// Navigate backwards
            await Shell.Current.GoToAsync("..");
        }
    }
}