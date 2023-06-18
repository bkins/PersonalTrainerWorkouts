﻿using System;
using System.Linq;
using Avails.D_Flat.Exceptions;
using Avails.D_Flat.Extensions;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Views.Tab_Workouts
{
    public partial class WorkoutEntryPage
    {
        private WorkoutEntryViewModel ViewModel { get; set; }
        private Entry                 NameEntry { get; set; }

        public WorkoutEntryPage()
        {
            InitializeComponent();
            ViewModel      = new WorkoutEntryViewModel();
            BindingContext = ViewModel;

            //NameEntry.Focus();

            DifficultyEditor.Text = "0";
        }

        //https://docs.microsoft.com/en-us/xamarin/get-started/quickstarts/database?pivots=windows

        private void OnSelectionChanged(object                    sender
                                            , SelectionChangedEventArgs e)
        {
            var exercise = (Exercise)e.CurrentSelection.FirstOrDefault();

            if (exercise == null)
            {
                return;
            }

            PageNavigation.NavigateTo(nameof(ExerciseAddEditPage)
                                          , nameof(ExerciseAddEditPage.WorkoutId)
                                          , ViewModel.NewWorkout.Id.ToString()
                                          , nameof(ExerciseAddEditPage.ExerciseId)
                                          , exercise.Id.ToString());
        }

        void OnManageExercisesClicked(object    sender
                                          , EventArgs e)
        {
            PageNavigation.NavigateTo(nameof(ExerciseListPage)
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

        private void Difficulty_OnUnfocused(object         sender
                                                , FocusEventArgs e)
        {
            var difficulty = (Entry)sender;

            if (difficulty.Text.HasValue())
            {
                SaveWorkout();
            }

            PageNavigation.NavigateTo(nameof(WorkoutListPage));
        }
    }
}
