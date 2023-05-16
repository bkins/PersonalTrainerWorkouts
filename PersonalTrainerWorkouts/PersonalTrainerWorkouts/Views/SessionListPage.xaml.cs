using Avails.D_Flat.Extensions;
using Avails.Xamarin;
using Avails.Xamarin.Logger;

using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels;

using Syncfusion.ListView.XForms;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionListPage
    {
        public int SwipedItem { get; set; }
        public SessionListViewModel ViewModel { get; set; }

        public SessionListPage()
        {
            InitializeComponent();

            ShowBusyIndicator(true
                            , "Initialing...");
        }

        protected override void OnAppearing()
        {
            ShowBusyIndicator(true
                            , "Initialing...");

            ViewModel = new SessionListViewModel();

            ListView.ItemsSource = ViewModel.ObservableListOfSessions;

            ShowBusyIndicator(false);
        }

        private void AddToolbarItem_OnClicked(object sender
                                            , EventArgs e)
        {
            PageNavigation.NavigateTo(nameof(SessionEditPage)
                                    , nameof(SessionEditPage.SessionId)
                                    , "0");
        }

        private void SearchToolbarItem_OnClicked(object sender
                                               , EventArgs e)
        {
            SessionsFilter.IsVisible = !SessionsFilter.IsVisible;
        }

        private void SessionsFilter_OnTextChanged(object sender
                                                , TextChangedEventArgs e)
        {
            ListView.ItemsSource = ViewModel.SearchSessions(SessionsFilter.Text);
        }

        private void OnSelectionChanged(object sender
                                      , ItemSelectionChangedEventArgs e)
        {
            if (e.AddedItems == null)
                return;

            var item = (Session)e.AddedItems.FirstOrDefault();

            if (item == null)
            {
                return;
            }
            PageNavigation.NavigateTo(nameof(SessionEditPage)
                                    , nameof(SessionEditPage.SessionId)
                                    , item.Id.ToString());

            ListView.SelectedItems.Clear();

        }

        private void ListView_SwipeEnded(object sender
                                       , SwipeEndedEventArgs e)
        {
            SwipedItem = e.ItemIndex;
        }

        private void LeftImage_BindingContextChanged(object sender
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
        private void Delete()
        {
            var itemDeleted = ViewModel.Delete(SwipedItem);

            if (!itemDeleted.success)
            {
                Logger.WriteLine("Session could not be deleted.  Please try again."
                               , Category.Warning);
            }

            ListView.ItemsSource = ViewModel.ObservableListOfSessions;

            Logger.WriteLine($"Deleted Session: {itemDeleted.item} deleted."
                           , Category.Information);

            ListView.ResetSwipe();
        }

        private async void ShowBusyIndicator(bool show, string titleMessage = "")
        {
            if (!show)
                await Task.Delay(600);

            Device.BeginInvokeOnMainThread(() =>
            {
                if (titleMessage.HasValue()) BusyIndicator.Title = titleMessage;

                BusyIndicator.EnableAnimation = show;
                BusyIndicator.IsVisible = show;
            });
        }
    }
}