using System;
using System.Linq;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Utilities;
using PersonalTrainerWorkouts.ViewModels;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    public partial class WorkoutListPage : ContentPage
    {
        public int                  SwipedItem { get; set; }
        public WorkoutListViewModel ViewModel  { get; set; }

        public WorkoutListPage()
        {
            InitializeComponent();
            ViewModel = new WorkoutListViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ListView.ItemsSource = ViewModel.ListOfWorkouts;
        }

        async void OnAddClicked(object    sender
                              , EventArgs e)
        {
            await PageNavigation.NavigateTo(nameof(WorkoutEntryPage));
        }

        async void OnSelectionChanged(object                        sender
                                    , ItemSelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
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
                (deleteImage.Parent as View)?.GestureRecognizers.Add(new TapGestureRecognizer
                                                                     {
                                                                         Command = new Command(Delete)
                                                                     });
            }
        }

        private void ListView_SwipeEnded(object              sender
                                       , SwipeEndedEventArgs swipeEndedEventArgs)
        {
            SwipedItem = swipeEndedEventArgs.ItemIndex;
        }

        private void Delete()
        {
            var itemDeleted = ViewModel.Delete(SwipedItem);

            if (itemDeleted == string.Empty)
            {
                Logger.WriteLine("Workout could not be deleted.  Please try again."
                               , Category.Warning);
            }

            ListView.ItemsSource = ViewModel.ListOfWorkouts;

            Logger.WriteLine($"Deleted Workout: {itemDeleted} deleted."
                           , Category.Information);

            ListView.ResetSwipe();
        }
    }
}
