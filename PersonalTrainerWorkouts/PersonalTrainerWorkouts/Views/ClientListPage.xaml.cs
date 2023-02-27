using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels;
using Syncfusion.ListView.XForms;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientListPage : ContentPage
    {
        public int                 SwipedItem { get; set; }
        public ClientListViewModel ViewModel  { get; set; }

        public ClientListPage()
        {
            InitializeComponent();
            ViewModel = new ClientListViewModel();
        }
        protected override void OnAppearing()
        {
            //ViewModel.SyncContacts();
            ViewModel.LoadData();
            
            ListView.ItemsSource = ViewModel.ObservableClients;
        }
        
        private async void AddToolbarItem_OnClicked(object    sender
                                                  , EventArgs e)
        {
            var clientAdded = await ViewModel.AddNewClient()
                                             .ConfigureAwait(false);
            
            if ( ! clientAdded)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Did you pick a contact?"
                                     , "Either you did not pick a contact or something went horribly wrong. If you did not select a contact than ignore this. Otherwise, see Logs for more details."
                                     , "OK")
                    .ConfigureAwait(false);
                });
                
                return;
            }

            Device.BeginInvokeOnMainThread( () =>
            {
                ListView.ItemsSource = ViewModel.ObservableClients;
            });

        }

        private void SearchToolbarItem_OnClicked(object    sender
                                               , EventArgs e)
        {
            Filter.IsVisible = ! Filter.IsVisible;
        }

        private void Filter_OnTextChanged(object               sender
                                        , TextChangedEventArgs e)
        {
            ListView.ItemsSource = ViewModel.SearchClients(Filter.Text);
        }

        private void OnSelectionChanged(object                        sender
                                      , ItemSelectionChangedEventArgs e)
        {
            var item = (Client) e.AddedItems?.FirstOrDefault();

            if (item == null) { return; }

            ListView.SelectedItems.Clear();

            PageNavigation.NavigateTo(nameof(ClientEditPage)
                                    , nameof(ClientEditPage.ClientId)
                                    , item.ClientId.ToString());
        }

        private void ListView_SwipeEnded(object              sender
                                       , SwipeEndedEventArgs e)
        {
            SwipedItem = e.ItemIndex;
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
        
        private void Delete()
        {
            var itemDeleted = ViewModel.Delete(SwipedItem);

            if ( ! itemDeleted.success)
            {
                Logger.WriteLine("Client could not be deleted.  Please try again."
                               , Category.Warning);
            }

            ListView.ItemsSource = ViewModel.ObservableClients;

            Logger.WriteLine($"Deleted Client: {itemDeleted.item} deleted."
                           , Category.Information);

            ListView.ResetSwipe();
        }
    }
}