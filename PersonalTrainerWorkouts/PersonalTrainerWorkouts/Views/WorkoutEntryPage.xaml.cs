using System;
using System.Linq;
using ApplicationExceptions;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels;
using PersonalTrainerWorkouts.Utilities;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Views
{
    public partial class WorkoutEntryPage : ContentPage
    {
        private WorkoutEntryViewModel ViewModel { get; set; }
        private Entry                 NameEntry { get; set; }

        public WorkoutEntryPage()
        {
            InitializeComponent();
            ViewModel      = new WorkoutEntryViewModel();
            BindingContext = ViewModel;

            //NameEntry.Focus();

            DifficultyEditor.Text = string.Empty;
        }

        //https://docs.microsoft.com/en-us/xamarin/get-started/quickstarts/database?pivots=windows

        private async void OnSelectionChanged(object                    sender
                                            , SelectionChangedEventArgs e)
        {
            var exercise = (Exercise)e.CurrentSelection.FirstOrDefault();

            if (exercise == null)
            {
                return;
            }

            await PageNavigation.NavigateTo(nameof(ExerciseAddEditPage)
                                          , nameof(ExerciseAddEditPage.WorkoutId)
                                          , ViewModel.NewWorkout.Id.ToString()
                                          , nameof(ExerciseAddEditPage.ExerciseId)
                                          , exercise.Id.ToString());
        }

        async void OnManageExercisesClicked(object    sender
                                          , EventArgs e)
        {
            await PageNavigation.NavigateTo(nameof(ExerciseListPage)
                                          , nameof(ExerciseListPage.WorkoutId)
                                          , ViewModel.NewWorkout.Id.ToString());
        }

        private void SaveWorkout()
        {
            var context = (WorkoutEntryViewModel)BindingContext;
            context.NewWorkout.CreateDateTime = DateTime.UtcNow;

            try
            {
                context.SaveWorkout();
            }
            catch (AttemptToAddDuplicateEntityException e)
            {
                NameEntry.Focus();

                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);
            }
            catch (UnnamedEntityException e)
            {
                NameEntry.Focus();

                Logger.WriteLine("Please name the workout before continuing."
                               , Category.Warning
                               , e);
            }
        }

        private void Name_OnUnfocused(object         sender
                                    , FocusEventArgs e)
        {
            NameEntry = (Entry)sender;

            SaveWorkout();
        }

        private void Description_OnUnfocused(object         sender
                                           , FocusEventArgs e)
        {
            var description = (Editor)sender;

            if (description.Text.HasValue())
            {
                SaveWorkout();
            }
        }

        private async void Difficulty_OnUnfocused(object         sender
                                                , FocusEventArgs e)
        {
            var difficulty = (Entry)sender;

            if (difficulty.Text.HasValue())
            {
                SaveWorkout();
            }

            await PageNavigation.NavigateTo(nameof(WorkoutListPage));
        }
    }
}
