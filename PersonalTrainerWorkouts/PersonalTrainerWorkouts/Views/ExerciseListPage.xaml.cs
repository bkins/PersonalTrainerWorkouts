using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.Views
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class ExerciseListPage : ContentPage
    {
        private string _itemId = "0";

        public string ItemId
        {
            get => _itemId;
            set => LoadExercises(value);
        }
        
       
        async void LoadExercises(string itemId)
        {
            try
            {
                _itemId = itemId;
                var id = Convert.ToInt32(itemId);
                CollectionView.ItemsSource = await App.Database.GetExercisesInWorkoutAsync(id);
            }
            catch (Exception)
            {
                //BENDO:  Implement "toast" messages: https://stackoverflow.com/a/44126899/431319
                Console.WriteLine("Failed to load Workout.");
            }
        }

        public ExerciseListPage()
        {
            InitializeComponent();
        }
        
        private async void OnAddClicked(object    sender,
                                        EventArgs e)
        {
            var path = $"{nameof(ExerciseNewEntryPage)}?{nameof(ExerciseNewEntryPage.WorkoutId)}={ItemId}";
            await Shell.Current.GoToAsync(path);
        }

        private async void OnExistingClicked(object    sender,
                                             EventArgs e)
        {
            var path = $"{nameof(ExerciseExistingEntryPage)}?{nameof(ExerciseExistingEntryPage.ItemId)}={0}";
            await Shell.Current.GoToAsync(path);
        }

        private async void OnSelectionChanged(object                    sender,
                                              SelectionChangedEventArgs e)
        {
            var path = $"{nameof(ExerciseExistingEntryPage)}?{nameof(ExerciseExistingEntryPage.ItemId)}={ItemId}";
            await Shell.Current.GoToAsync(path);
        }

    }
}