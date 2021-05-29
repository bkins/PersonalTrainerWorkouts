using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PersonalTrainerWorkouts.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views
{
    
    public partial class InitialPage : ContentPage
    {
        public InitialPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Retrieve all the notes from the database, and set them as the
            // data source for the CollectionView.
            CollectionView.ItemsSource = await App.Database.GetWorkoutsAsync();
        }

        async void OnAddClicked(object    sender,
                                  EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(WorkoutEntryPage));
        }

        async void OnSelectionChanged(object                    sender,
                                      SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                // Navigate to the NoteEntryPage, passing the ID as a query parameter.
                Workouts workout = (Workouts)e.CurrentSelection.FirstOrDefault();
                var      path    = $"{nameof(WorkoutEntryPage)}?{nameof(WorkoutEntryPage.ItemId)}={workout.Id}";
                await Shell.Current.GoToAsync(path);
            }
        }
    }
}