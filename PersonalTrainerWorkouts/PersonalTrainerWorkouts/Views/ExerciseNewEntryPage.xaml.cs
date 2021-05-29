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
    [QueryProperty(nameof(WorkoutId), nameof(WorkoutId))]
    [QueryProperty(nameof(ExerciseId), nameof(ExerciseId))]
    public partial class ExerciseNewEntryPage : ContentPage, INotifyPropertyChanged
    {
        
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

        private async void LoadExercise(string id)
        {
            try
            {
                _exerciseId = id;

                var workout = await App.Database.GetWorkoutsAsync(_workoutId);

                var exercise = workout.Exercises.FirstOrDefault(e => e.Id == Convert.ToInt32(_exerciseId));
                
                BindingContext = exercise;
            }
            catch (Exception)
            {
                //BENDO:  Implement "toast" messages: https://stackoverflow.com/a/44126899/431319
                Console.WriteLine("Failed to load Workout.");
            }
        }

        private void LoadWorkout(string id)
        {
            _workoutId = id;
        }

        public ExerciseNewEntryPage()
        {
            InitializeComponent();
            BindingContext = new Exercises();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            //AllExercises = await App.Database.GetAllExercisesAsync();
            //SetAllExercises();
            //BindingContext = App.Database.GetAllExercisesAsync()
            //    .Result;
        }

        private async void SaveWorkout()
        {
            var exercise = await GetExercise();

            if (!string.IsNullOrWhiteSpace(exercise.Name))
            {
                var workout = App.Database.GetWorkoutsAsync(Convert.ToInt32(WorkoutId)).Result;
                workout.Exercises.Add(exercise);
                await App.Database.SaveWorkoutAsync(workout);

                // Navigate backwards
                await Shell.Current.GoToAsync("..");
            }
        }

        /// <summary>
        /// Gets exercise entered by the user, or if the exercise the user is entering already exists,
        /// the user will be if add the existing one.  If they say no, then the user will be able to edit
        /// the exercise they are trying to enter. 
        /// </summary>
        /// <returns></returns>
        private async Task<Exercises> GetExercise()
        {
            var exercise         = (Exercises) BindingContext;
            var existingExercise = GetExistingExercise(exercise);

            if (existingExercise    == null ||
                existingExercise.Id == 0)
            {
                return exercise;
            }

            var yes = await DisplayAlert("Exercise already exists", "Would you like to add the existing exercise to the workout instead?", "Yes", "No");

            if ( ! yes)
            {
                return exercise;
            }
            
            if (DoesExerciseExistInWorkout(existingExercise))
            {
                await DisplayAlert("Can't add this exercise to this workout", $"The exercise {existingExercise.Name} is already in the workout", "OK");
                existingExercise.Name = string.Empty;
            }

            exercise = existingExercise;

            return exercise;
        }

        private bool DoesExerciseExistInWorkout(Exercises existingExercise)
        {
            var workout                    = App.Database.GetWorkoutsAsync(WorkoutId);
            var doesExerciseExistInWorkout = workout.Result.Exercises.Any(e => e.Name == existingExercise.Name);
            return doesExerciseExistInWorkout;
        }

        private static Exercises GetExistingExercise(Exercises exercise)
        {
            var existingExercise = App.Database.GetExerciseByName(exercise.Name);
            
            return existingExercise.Result;
        }

        void OnSaveButtonClicked(object    sender,
                                 EventArgs e)
        {
            SaveWorkout();
        }
        
        private void OnDeleteButtonClicked(object    sender,
                                           EventArgs e)
        {
            var exercise = (Exercises) BindingContext;
            App.Database.DeleteExerciseAsync(exercise);
        }
        
    }
}