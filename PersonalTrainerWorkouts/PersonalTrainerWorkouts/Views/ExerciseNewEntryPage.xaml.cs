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
        //private ExerciseNewEntryViewModel ViewModel { get; set; }
        private ExerciseAddEditViewModel ViewModel           { get; set; }
        public  string                   WorkoutId           { get; set; }
        public  string                   ExerciseId          { get; set; }
        public  string                   InitialName         { get; set; }
        public  string                   InitialDescription  { get; set; }
        public  string                   InitialLengthOfTime { get; set; }
        private Entry                    NameEntry           { get; set; }
        
        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            try
            {
                // The query parameter requires URL decoding.
                WorkoutId  = HttpUtility.UrlDecode(query[nameof(WorkoutId)]);
                ExerciseId = HttpUtility.UrlDecode(query[nameof(ExerciseId)]);

                ViewModel = new ExerciseAddEditViewModel(int.Parse(WorkoutId)
                                                       , int.Parse(ExerciseId));
                LoadData();
            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed initiate ExerciseNewEntryPage.", Category.Error, e);

                throw;
            }
        }

        private void LoadData()
        {
            try
            {
                InitialName         = ViewModel.Exercise.Name;
                InitialDescription  = ViewModel.Exercise.Description;
                InitialLengthOfTime = ViewModel.Exercise.LengthOfTime;

                BindingContext      = ViewModel;
            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed to load Workout.", Category.Error, e);
                //BENDO: consider implementing a page that shows exception details
            }
        }
        

        public ExerciseNewEntryPage()
        {
            InitializeComponent();
            ViewModel      = new ExerciseAddEditViewModel();
            BindingContext = ViewModel;
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
            ViewModel.Exercise = GetContextExercise();
            ViewModel.UpdateExercise();
        }

        private void SaveExercise()
        {
            ViewModel.Exercise = GetContextExercise();

            try
            {
                ViewModel.SaveExercise(Convert.ToInt32(WorkoutId));
            }
            catch (AttemptToAddDuplicateEntityException e)
            {
                Logger.WriteLine($"An exercise with the name {ViewModel.Exercise.Name} already exists.  Please either add that exercise or use a different name."
                               , Category.Warning
                               , e);

                NameEntry.Focus();
            }
            catch (UnnamedEntityException e)
            {
                Logger.WriteLine("An exercise must have a name to save.", Category.Warning, e);
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
            var exercise = (ExerciseAddEditViewModel) BindingContext;
            exercise.Exercise.LengthOfTime = exercise.LengthOfTime;

            return exercise.Exercise;
        }
        
        void OnSaveButtonClicked(object    sender,
                                 EventArgs e)
        {
            
        }
        
        private void Name_OnUnfocused(object         sender
                                    , FocusEventArgs e)
        {
            NameEntry = (Entry) sender;

            if (string.IsNullOrEmpty(InitialName))
            {
                SaveExercise();
            }
            else if (NameEntry.Text != InitialName)
            {
                UpdateExercise();
            }
        }

        private void Description_OnUnfocused(object         sender
                                           , FocusEventArgs e)
        {
            var description = (Editor) sender;

            if (InitialDescription != description.Text)
            {
                UpdateExercise();
            }
        }

        private async void LengthOfTimeEditor_OnUnfocused(object         sender
                                                        , FocusEventArgs e)
        {
            var lengthOfTime = (Entry) sender;

            if (InitialLengthOfTime != lengthOfTime.Text)
            {
                UpdateExercise();
            }

            await PageNavigation.NavigateBackwards();
        }
    }
}