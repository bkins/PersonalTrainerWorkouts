using System;
using System.Linq;
using ApplicationExceptions;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.Utilities;
using PersonalTrainerWorkouts.ViewModels;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Views
{
    [QueryProperty(nameof(WorkoutId), nameof(WorkoutId))]
    public partial class WorkoutExercisePage : ContentPage
    {
        public WorkoutsToExerciseViewModel ViewModel;
        public string                      TotalWorkoutTime { get; set; }

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
                
                var exercises = ViewModel.ExercisesWithIntermediateFields;

                BindingContext             = ViewModel;
                CollectionView.ItemsSource = exercises;
                TotalWorkoutTime           = ViewModel.TotalTime.ToShortForm();
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
        
        private void SaveWorkout()
        {
            var boundContext = (WorkoutsToExerciseViewModel) BindingContext;
            var workout      = boundContext.Workout;
            workout.CreateDateTime = DateTime.UtcNow;

            if ( string.IsNullOrWhiteSpace(workout.Name))
            {
                Logger.WriteLine("You must name the workout first, before saving", Category.Information);
                //BENDO: Display that workout was not save because a name was not provided
                //Or select the name field and highlight is red
            }
            else
            {
                ViewModel.SaveWorkout();
            }
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
            
            await PageNavigation.NavigateTo(nameof(ExerciseAddEditPage)
                                          , nameof(ExerciseAddEditPage.WorkoutId)
                                          , WorkoutId
                                          , nameof(ExerciseAddEditPage.ExerciseId)
                                          , exercise.Exercise.Id.ToString());
        }
        
        async void OnToolbarManageExercisesClicked(object    sender,
                                                   EventArgs e)
        {
            await PageNavigation.NavigateTo(nameof(ExerciseListPage)
                                          , nameof(ExerciseListPage.WorkoutId)
                                          , WorkoutId);
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
            //SelectedExerciseViewModel is only set when the Exercise item is selected, but when the LengthOfTime or Reps fields are select the exercise item is not
            //I can't get this to work :-(  Will use SaveWorkoutsToExercise button for now
            //var workoutExercise = ViewModel.WorkoutExercises
            //                               .First(field => field.Id == SelectedExerciseViewModel.WorkoutExerciseId);
            //App.Database.UpdateWorkoutExercises(workoutExercise);
        }
        
        private void ExerciseReps_OnUnfocused(object         sender
                                            , FocusEventArgs e)
        {
            //Silent save is not working.  See ExerciseLengthOfTime_OnUnfocused.  Use SaveWorkoutsToExercise button on each item in the Exercise List
        }

        private void OnSaveWorkoutExerciseButtonClick(object    sender
                                                    , EventArgs e)
        {
            var itemData = (Button) sender;

            if ( ! (itemData.CommandParameter is ExerciseViewModel selectedWorkoutExercise))
                return;

            var workoutsToExercise = BuildWorkoutToExerciseFromSelected(selectedWorkoutExercise);

            SaveWorkoutToExerciseAndReload(workoutsToExercise);

        }

        private void SaveWorkoutToExerciseAndReload(LinkedWorkoutsToExercises workoutsToExercise)
        {
            try
            {
                ViewModel.SaveWorkoutsToExercise(workoutsToExercise);
                LoadWorkout(ViewModel.Workout.Id.ToString());
            }
            catch (ValueTooLargeException exception)
            {
                Logger.WriteLine($"{exception.Message}  Value NOT saved!"
                               , Category.Warning
                               , exception);
            }
            catch (Exception exception)
            {
                Logger.WriteLine($"Error while saving the {typeof(LinkedWorkoutsToExercises)}: {exception.Message}"
                               , Category.Error
                               , exception);
            }
        }

        private LinkedWorkoutsToExercises BuildWorkoutToExerciseFromSelected(ExerciseViewModel selectedWorkoutExercise)
        {
            var workoutsToExercise = ViewModel.WorkoutsToExercises.First(field => field.Id == selectedWorkoutExercise.WorkoutExerciseId);

            workoutsToExercise.LengthOfTime = selectedWorkoutExercise.LengthOfTime;
            workoutsToExercise.Reps         = selectedWorkoutExercise.Reps;

            return workoutsToExercise;
        }
    }
}