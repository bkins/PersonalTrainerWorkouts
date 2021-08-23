using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.Utilities;
using PersonalTrainerWorkouts.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views
{
    [QueryProperty(nameof(WorkoutId), nameof(WorkoutId))]
    public partial class WorkoutExercisePage : ContentPage
    {
        public WorkoutsToExerciseViewModel ViewModel;
        public int                      TotalWorkoutTime { get; set; }

        private ExerciseViewModel     SelectedExerciseViewModel { get; set; }
        
        private string _workoutId = "0";
        public string WorkoutId
        {
            get => _workoutId;
            set => LoadWorkout(value);
        }

        public void LoadWorkout(string workoutId)
        {
            try
            {
                _workoutId = workoutId;

                ViewModel = new WorkoutsToExerciseViewModel(workoutId);

                var workout   = ViewModel.Workout;
                var exercises = ViewModel.ExercisesWithIntermediateFields;
                var totalTime = ViewModel.TotalTime;

                BindingContext             = ViewModel;
                CollectionView.ItemsSource = exercises;
                TotalWorkoutTime           = ViewModel.TotalTime;
                
            }
            catch (Exception ex)
            {

                Logger.WriteLine("Failed to load Workout.", Category.Error, ex);
                //BENDO: consider implementing a page that shows exception details
            }
        }

        public WorkoutExercisePage()
        {
            InitializeComponent();

            BindingContext = new Workout(); 
        }
        //https://docs.microsoft.com/en-us/xamarin/get-started/quickstarts/database?pivots=windows

        private void SaveWorkout()
        {
            var workout = ((WorkoutsToExerciseViewModel)BindingContext).Workout;
            workout.CreateDateTime = DateTime.UtcNow;

            if ( ! string.IsNullOrWhiteSpace(workout.Name))
            {
                Logger.WriteLine("You must name the workout first, before saving", Category.Information);
                //BENDO: Display that workout was not save because a name was not provided
                //Or select the name field and highlight is red
                App.Database.SaveWorkout(workout);
            }
        }

        private void SaveExercise()
        {

        }
        
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var workout = (Workout)BindingContext;
            workout.CreateDateTime = DateTime.UtcNow;
            if ( ! string.IsNullOrWhiteSpace(workout.Name))
            {
                //await App.AsyncDatabase.SaveWorkoutAsync(workout);
                App.Database.SaveWorkout(workout);
            }

            // Navigate backwards
            await Shell.Current.GoToAsync("..");
        }
            
        private async void OnSelectionChanged(object                    sender,
                                              SelectionChangedEventArgs e)
        {
            var exercise = (ExerciseViewModel)e.CurrentSelection.FirstOrDefault();
            SelectedExerciseViewModel = exercise;

            if (exercise == null)
            {
                return;
            }

            //Navigate to Exercise page to edit existing exercise
            await PageNavigation.NavigateTo
                                 (
                                       nameof(ExerciseNewEntryPage)
                                     , nameof(ExerciseNewEntryPage.WorkoutId)
                                     , WorkoutId
                                     , nameof(ExerciseNewEntryPage.ExerciseId)
                                     , exercise.Exercise.Id.ToString()
                                 );
        }
        
        async void OnToolbarManageExercisesClicked(object    sender,
                                                   EventArgs e)
        {
            var path = $"{nameof(ExerciseListPage)}?{nameof(ExerciseListPage.WorkoutId)}={WorkoutId}";
            await Shell.Current.GoToAsync(path);
        }
        
        private void Name_OnUnfocused(object         sender
                                    , FocusEventArgs e)
        {
            SaveWorkout();
        }

        private void Description_OnUnfocused(object         sender
                                           , FocusEventArgs e)
        {
            SaveWorkout();
        }

        private void Difficulty_OnUnfocused(object         sender
                                          , FocusEventArgs e)
        {
            SaveWorkout();
        }

        private void ExerciseLengthOfTime_OnUnfocused(object         sender
                                                    , FocusEventArgs e)
        {
            var theSender = sender;
            var theE      = e;


            //BENDO: SelectedExerciseViewModel is null
            //I can't get this to work :-(
            //var workoutExercise = ViewModel.WorkoutExercises
            //                               .First(field => field.Id == SelectedExerciseViewModel.WorkoutExerciseId);
            //App.Database.UpdateWorkoutExercises(workoutExercise);
        }
        
        private void ExerciseReps_OnUnfocused(object         sender
                                            , FocusEventArgs e)
        {
            //Silent save is not working.  See ExerciseLengthOfTime_OnUnfocused.  Use Save button on each item in the Exercise List
        }

        private void OnSaveWorkoutExerciseButtonClick(object    sender
                                                    , EventArgs e)
        {
            var itemData = (Button) sender;

            if ( ! (itemData.CommandParameter is ExerciseViewModel selectedWorkoutExercise))
                return;

            var workoutsToExercise = ViewModel.WorkoutsToExercises
                                              .First(field => field.Id == selectedWorkoutExercise.WorkoutExerciseId);
            
            workoutsToExercise.LengthOfTime = selectedWorkoutExercise.LengthOfTime;
            workoutsToExercise.Reps         = selectedWorkoutExercise.Reps;

            ViewModel.Save(workoutsToExercise);
            LoadWorkout(ViewModel.Workout.Id.ToString());
        }

    }
}