using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ApplicationExceptions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Utilities;
using PersonalTrainerWorkouts.ViewModels;

namespace PersonalTrainerWorkouts.Views
{
    [QueryProperty(nameof(WorkoutId),  nameof(WorkoutId))]
    [QueryProperty(nameof(ExerciseId), nameof(ExerciseId))]
    public partial class ExerciseNewEntryPage : ContentPage, INotifyPropertyChanged, IQueryAttributable
    {
        
        private string                    _workoutId = "0";
        private ExerciseNewEntryViewModel ViewModel { get; set; }
        private Entry                     NameEntry { get; set; }

        public  string                    WorkoutId { get; set; }
        //{
        //    get => _workoutId;
        //    set => LoadWorkout(value);
        //}

        private string _exerciseId = "0";

        public string ExerciseId { get; set; }
        //{
        //    get => _exerciseId;
        //    set => LoadExercise(value);
        //}
        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            try
            {
                // The query parameter requires URL decoding.
                WorkoutId  = HttpUtility.UrlDecode(query[nameof(WorkoutId)]);
                ExerciseId = HttpUtility.UrlDecode(query[nameof(ExerciseId)]);
                LoadExercise();
            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed initiate ExerciseNewEntryPage.", Category.Error, e);

                throw;
            }
        }
        private async void LoadExercise()
        {
            try
            {
                //var workout  = await App.AsyncDatabase.GetWorkoutsAsync(WorkoutId);
                var workout = App.Database.GetWorkout(Convert.ToInt32(WorkoutId));

                var exercise = workout.Exercises.FirstOrDefault(e => e.Id == Convert.ToInt32(ExerciseId));
                
                BindingContext = exercise ?? new Exercise();

                LengthOfTimeEditor.Text = LengthOfTimeEditor.Text == "0" ?
                                                  "" :
                                                  LengthOfTimeEditor.Text;
            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed to load Workout.", Category.Error, e);
                //BENDO: consider implementing a page that shows exception details
            }
        }

        private void LoadWorkout(string id)
        {
            _workoutId = id;
        }

        public ExerciseNewEntryPage()
        {
            InitializeComponent();
            ViewModel      = new ExerciseNewEntryViewModel();
            BindingContext = ViewModel.NewExercise;
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            //AllExercises = await App.Database.GetAllExercisesAsync();
            //SetAllExercises();
            //BindingContext = App.Database.GetAllExercisesAsync()
            //    .Result;
        }

        private void UpdateExercise()
        {
            ViewModel.NewExercise = GetContextExercise();
            ViewModel.UpdateNewExercise();
        }

        private async void SaveExercise()
        {
            ViewModel.NewExercise = GetContextExercise();

            try
            {
                ViewModel.SaveExercise(Convert.ToInt32(WorkoutId));
            }
            catch (AttemptToAddDuplicateEntityException e)
            {
                Logger.WriteLine($"An exercise with the name {ViewModel.NewExercise.Name} already exists.  Please either add that exercise or use a different name."
                               , Category.Warning
                               , e);

                NameEntry.Focus();
            }
            catch (UnnamedEntityException e)
            {
                Logger.WriteLine("An exercise must have a name to save.", Category.Warning, e);
            }
            
            return;

            //BENDO: Replace this with Save in ViewModel
            try
            {
                var exercise = GetContextExercise();
                
                if ( ! string.IsNullOrWhiteSpace(exercise.Name))
                {
                    //BENDO: Interfacing with the database needs to be in the ViewModel.  Move this to the viewModel.
                    var workout = App.Database.GetWorkout(Convert.ToInt32(WorkoutId));
                    
                    workout.Exercises.Add(exercise);
                    
                    //BENDO: Interfacing with the database needs to be in the ViewModel.  Move this to the viewModel.
                    App.Database.AddCompleteWorkout(workout);
                    
                    // Navigate backwards
                    await PageNavigation.NavigateBackwards();
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message, Category.Error, e);

                throw;
            }
        }

        /// <summary>
        /// Gets exercise entered by the user, or if the exercise the user is entering already exists,
        /// the user will be if add the existing one.  If they say no, then the user will be able to edit
        /// the exercise they are trying to enter. 
        /// </summary>
        /// <returns></returns>
        private Exercise GetContextExercise()
        {
            var exercise = (Exercise) BindingContext;
            return exercise;

            //BUG:  Database call to get exercise just hangs when connected to actual device.
            //Behavior does not occur when using emulator.  Need more testing!
            //For now, I am not going to implement prevention of adding duplicate exercises to a Workout

            //Logger.WriteLine($"Current exercise: ID={exercise.Id}; Name={exercise.Name}"
            //               , Category.Information);

            //var existingExercise = GetExistingExercise(exercise);
            
            //if (existingExercise    == null ||
            //    existingExercise.Id == 0)
            //{
            //    return exercise;
            //}

            //var yes = await DisplayAlert("Exercise already exists", "Would you like to add the existing exercise to the workout instead?", "Yes", "No");

            //if ( ! yes)
            //{
            //    return exercise;
            //}
            
            //if (DoesExerciseExistInWorkout(existingExercise))
            //{
            //    await DisplayAlert("Can't add this exercise to this workout", $"The exercise {existingExercise.Name} is already in the workout", "OK");
            //    existingExercise.Name = string.Empty;
            //}

            //exercise = existingExercise;

            //return exercise;
        }

        //Obsolete:  On adding to the database duplicate names are validated
        //private bool DoesExerciseExistInWorkout(Exercise existingExercise)
        //{
        //    //var workout                    = App.AsyncDatabase.GetWorkoutsAsync(WorkoutId);
        //    var workout                    = App.Database.GetWorkout(Convert.ToInt32(WorkoutId));
        //    var doesExerciseExistInWorkout = workout.Exercises.Any(field => field.Name == existingExercise.Name);
        //    return doesExerciseExistInWorkout;
        //}

        //private static Exercise GetExistingExercise(Exercise exercise)
        //{
        //    var existingExercise = App.AsyncDatabase.GetExerciseByName(exercise.Name);
            
        //    return existingExercise.Result;
        //}

        void OnSaveButtonClicked(object    sender,
                                 EventArgs e)
        {
            
        }
        
        private void OnDeleteButtonClicked(object    sender,
                                           EventArgs e)
        {
            var exercise = (Exercise) BindingContext;
            //App.AsyncDatabase.DeleteExerciseAsync(exercise);
            App.Database.DeleteExercise(ref exercise);
        }

        private void OnToolbarDeleteClicked(object    sender
                                          , EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Name_OnUnfocused(object         sender
                                             , FocusEventArgs e)
        {
            NameEntry = (Entry) sender;
            SaveExercise();
        }

        private void Description_OnUnfocused(object         sender
                                             , FocusEventArgs e)
        {
            UpdateExercise();
        }

        private async void LengthOfTimeEditor_OnUnfocused(object         sender
                                                  , FocusEventArgs e)
        {
            UpdateExercise();
            await PageNavigation.NavigateBackwards();
        }
    }
}