using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Utilities;
using PersonalTrainerWorkouts.Utilities.Interfaces;
using PersonalTrainerWorkouts.ViewModels;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    
    public partial class InitialPage : ContentPage
    {
        public int                  SwipedItem     { get; set; }
        public InitialPageViewModel ViewModel      { get; set; }

        public InitialPage()
        {
            InitializeComponent();
            ViewModel = new InitialPageViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            ListView.ItemsSource = ViewModel.ListOfWorkouts;
        }
        
        async void OnAddClicked(object    sender,
                                EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(WorkoutEntryPage));
        }

        async void OnSelectionChanged(object                        sender,
                                      ItemSelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                // Navigate to the NoteEntryPage, passing the ID as a query parameter.
                var workout = (Workout)e.AddedItems.FirstOrDefault();
                
                if (workout == null)
                {
                    return;
                }

                await PageNavigation.NavigateTo(nameof(WorkoutExercisePage)
                                              , nameof(WorkoutExercisePage.WorkoutId)
                                              , workout.Id.ToString());
            }
        }

        private void LeftImage_BindingContextChanged(object    sender
                                                   , EventArgs e)
        {
            if (sender is Image deleteImage)
            {
                (deleteImage.Parent as View)?.GestureRecognizers.Add
                        (
                            new TapGestureRecognizer
                            {
                                Command = new Command(Delete)
                            }
                        );
            }
        }
        
        private void ListView_SwipeEnded(object sender, SwipeEndedEventArgs swipeEndedEventArgs)
        {
            SwipedItem = swipeEndedEventArgs.ItemIndex;
        }

        private void Delete()
        {
            
            var itemDeleted = ViewModel.Delete(SwipedItem);

            if (itemDeleted == string.Empty)
            {
                Logger.WriteLine("Workout could not be deleted.  Please try again.", Category.Warning);
            }

            ListView.ItemsSource = ViewModel.ListOfWorkouts;

            Logger.WriteLine($"Deleted Workout: {itemDeleted} deleted.", Category.Information);
            
            ListView.ResetSwipe();
        }

        private void ListView_OnSwiping(object           sender
                                      , SwipingEventArgs e)
        {
            
        }

        private void RightImage_BindingContextChanged(object    sender
                                                    , EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}