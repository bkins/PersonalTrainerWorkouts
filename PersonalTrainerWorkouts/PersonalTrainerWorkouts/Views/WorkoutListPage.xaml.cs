using System;
using System.Linq;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    public partial class WorkoutListPage
    {
        public int                  SwipedItem { get; set; }
        public WorkoutListViewModel ViewModel  { get; set; }

        public WorkoutListPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel = new WorkoutListViewModel();

            ListView.ItemsSource = ViewModel.ObservableListOfWorkouts;
        }

        // async Task OnAddClicked(object    sender
        //                       , EventArgs e)
        // {
        //     await PageNavigation.NavigateTo(nameof(WorkoutEntryPage));
        // }

        void OnSelectionChanged(object                        sender
                              , ItemSelectionChangedEventArgs e)
        {
            if (e.AddedItems == null)
                return;

            var workout = (Workout)e.AddedItems.FirstOrDefault();

            if (workout == null)
            {
                return;
            }

            ListView.SelectedItems.Clear();

            PageNavigation.NavigateTo(nameof(WorkoutExercisePage)
                                    , nameof(WorkoutExercisePage.WorkoutId)
                                    , workout.Id.ToString());
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

            ListView.ItemsSource = ViewModel.ObservableListOfWorkouts;

            Logger.WriteLine($"Deleted Workout: {itemDeleted} deleted."
                           , Category.Information);

            ListView.ResetSwipe();
        }

        private void WorkoutFilter_OnTextChanged(object               sender
                                               , TextChangedEventArgs e)
        {
            ListView.ItemsSource = ViewModel.SearchWorkouts(WorkoutFilter.Text);
        }

        private void SearchToolbarItem_OnClicked(object    sender
                                               , EventArgs e)
        {
            WorkoutFilter.IsVisible = ! WorkoutFilter.IsVisible;
        }

        private void AddToolbarItem_OnClicked(object    sender
                                                  , EventArgs e)
        {
            PageNavigation.NavigateTo(nameof(WorkoutEntryPage));
        }
    }
}
