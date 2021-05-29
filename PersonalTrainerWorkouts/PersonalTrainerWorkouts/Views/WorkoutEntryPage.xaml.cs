using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalTrainerWorkouts.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class WorkoutEntryPage : ContentPage
    {
        private string _itemId = "0";

        public string ItemId
        {
            get => _itemId;
            set => LoadWorkout(value);
        }
        async void LoadWorkout(string itemId)
        {
            try
            {
                _itemId = itemId;

                var id      = Convert.ToInt32(itemId);
                var workout = await App.Database.GetWorkoutsAsync(id);
                
                BindingContext             = workout;
                CollectionView.ItemsSource = await App.Database.GetExercisesInWorkoutAsync(id);
            }
            catch (Exception)
            {
                //BENDO:  Implement "toast" messages: https://stackoverflow.com/a/44126899/431319
                Console.WriteLine("Failed to load Workout.");
            }
        }

        public WorkoutEntryPage()
        {
            InitializeComponent();

            BindingContext = new Workouts();
        }
        //https://docs.microsoft.com/en-us/xamarin/get-started/quickstarts/database?pivots=windows
        
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var workout = (Workouts)BindingContext;
            workout.CreateDateTime = DateTime.UtcNow;
            if ( ! string.IsNullOrWhiteSpace(workout.Title))
            {
                await App.Database.SaveWorkoutAsync(workout);
            }

            // Navigate backwards
            await Shell.Current.GoToAsync("..");
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var workout = (Workouts)BindingContext;
            await App.Database.DeleteWorkoutAsync(workout);

            // Navigate backwards
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSelectionChanged(object                    sender,
                                              SelectionChangedEventArgs e)
        {
            var exercise = (Exercises)e.CurrentSelection.FirstOrDefault();

            if (exercise == null)
            {
                return;
            }

            var path = $"{nameof(ExerciseNewEntryPage)}?{nameof(ExerciseNewEntryPage.WorkoutId)}={ItemId}&{nameof(ExerciseNewEntryPage.ExerciseId)}={exercise.Id}";
            await Shell.Current.GoToAsync(path);
        }
        
        async void OnManageExercisesClicked(object    sender,
                                            EventArgs e)
        {
            var path = $"{nameof(ExerciseListPage)}?{nameof(ExerciseListPage.ItemId)}={ItemId}";
            await Shell.Current.GoToAsync(path);
        }
    }
}