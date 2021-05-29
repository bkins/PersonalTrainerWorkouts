using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.Views
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class ExerciseExistingEntryPage : ContentPage, INotifyPropertyChanged
    {

        private string _itemId = "0";

        public string ItemId
        {
            get => _itemId;
            set => LoadExercises(value);
        }
        
        public List<Exercises> AllExercises { get; set; }  //=> App.Database.GetAllExercisesAsync().Result;
        
        private Exercises _selectedExercise;

        public Exercises SelectedExercise
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
                AllExercises = await App.Database.GetAllExercisesAsync();
            }
            catch (Exception)
            {
                //BENDO:  Implement "toast" messages: https://stackoverflow.com/a/44126899/431319
                Console.WriteLine("Failed to load Workout.");
            }
        }

        public ExerciseExistingEntryPage()
        {
            InitializeComponent();
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
            AllExercises = await App.Database.GetAllExercisesAsync();
        }

        async void OnSaveButtonClicked(object    sender,
                                       EventArgs e)
        {
            var exercise = (Exercises)BindingContext;
            if ( ! string.IsNullOrWhiteSpace(exercise.Name))
            {
                var workout = App.Database.GetWorkoutsAsync(Convert.ToInt32(ItemId)).Result;
                workout.Exercises.Add(exercise);
                await App.Database.SaveWorkoutAsync(workout);

                // Navigate backwards
                await Shell.Current.GoToAsync("..");
            }

        }

        private void OnDeleteButtonClicked(object    sender,
                                           EventArgs e)
        {
            throw new NotImplementedException();
        }
        
    }
}