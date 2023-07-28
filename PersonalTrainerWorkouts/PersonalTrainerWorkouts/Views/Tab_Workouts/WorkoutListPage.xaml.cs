using System;
using System.Linq;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using Avails.Xamarin.Utilities;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts;
using Syncfusion.ListView.XForms;
using Xamarin.Essentials;
using Xamarin.Forms;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;
using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;

namespace PersonalTrainerWorkouts.Views.Tab_Workouts
{
    public partial class WorkoutListPage
    {
        public int                  SwipedItem { get; set; }
        public WorkoutListViewModel ViewModel  { get; set; }

        private string selectedNumber { get; set; }

        public WorkoutListPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel = new WorkoutListViewModel();

            ListView.ItemsSource = ViewModel.ObservableListOfWorkouts;
            ClientPicker.ItemsSource = ViewModel.Clients;
        }

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

            var instance = new WorkoutExercisePage(workout.Id.ToString());

            PageNavigation.NavigateTo(instance);
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

        private void ShareImage_OnBindingContextChanged(object    sender
                                                      , EventArgs e)
        {
            UiUtilities.AddCommandToGestureToImage(sender, SendWorkoutToClient);
        }

        private void SendWorkoutToClient()
        {
            //Show client picker
            ClientPicker.IsOpen = true;
            ListView.ResetSwipe();

            //Get selected client
            //get workout into text format
            //send workout text via text
        }

        private void ClientPicker_OnOkButtonClicked(object                                               sender
                                                  , Syncfusion.SfPicker.XForms.SelectionChangedEventArgs selectionChangedEventArgs)
        {
            try
            {
                var client  = (Client)ClientPicker.SelectedItem;
                var workout = ViewModel.GetWorkoutByIndex(SwipedItem);

                Sms.ComposeAsync(new SmsMessage(workout.ToTextMessageString(client), client.MainNumber));
            }
            catch (Exception exception)
            {
                Logger.WriteLineToToastForced("Could not sent text message", Category.Error, exception);
            }
        }

        private void ClientPicker_OnCancelButtonClicked(object                    sender
                                                      , SelectionChangedEventArgs selectionChangedEventArgs)
        {

        }
    }
}
