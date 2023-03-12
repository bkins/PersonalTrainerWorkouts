using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientListPage
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
            try
            {
                ViewModel.LoadData();
                var loadedClients = new StringBuilder();
                foreach (var client in ViewModel.ObservableClients)
                {
                    var name   = client.DisplayName;
                    var number = client.MainNumber;

                    loadedClients.AppendLine($"{name} {number}");
                }
                
                Logger.WriteLine(loadedClients.ToString(), Category.Information);
                
                ListView.ItemsSource = ViewModel.ObservableClients;
            }
            catch (Exception e)
            {
                Logger.WriteLine("Problem loading client/contact data", Category.Error, e);
            }
            
        }
        
        private async void AddToolbarItem_OnClicked(object    sender
                                                  , EventArgs e)
        {
            // var progress = new Progress<string>(message => { BackUpDbButton.Text = message; });
            //
            // success = await Task.Run(() => BackupDbWork(progress))
            //                     .ConfigureAwait(false);
            //
            
            try
            {
                var clientAddedTask          = ViewModel.AddNewClient();
                clientAddedTask.Wait();
                var clientAdded = clientAddedTask.Result;
                //
                // var clientAdded = Task.Run(() => ViewModel.AddNewClient(progress)).ConfigureAwait(false);
                // await clientAdded;
            }
            catch (Exception exception)
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

            var d = 0;

            // var clientAdded = await ViewModel.AddNewClient()
            //                                  .ConfigureAwait(false);
            //
            // if ( ! clientAdded)
            // {
            //     Device.BeginInvokeOnMainThread(async () =>
            //     {
            //         await DisplayAlert("Did you pick a contact?"
            //                          , "Either you did not pick a contact or something went horribly wrong. If you did not select a contact than ignore this. Otherwise, see Logs for more details."
            //                          , "OK")
            //         .ConfigureAwait(false);
            //     });
            //     
            //     return;
            // }

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