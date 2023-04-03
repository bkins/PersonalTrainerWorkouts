using Avails.Xamarin;
using Avails.Xamarin.Logger;

using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels;

using Syncfusion.ListView.XForms;

using System;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientListPage
    {
        public int SwipedItem { get; set; }
        public ClientListViewModel clientListVm { get; set; }

        public ClientListPage()
        {
            InitializeComponent();
            clientListVm = new ClientListViewModel();
        }

        protected override void OnAppearing()
        {
            //ViewModel.SyncContacts();
            try
            {
                //clientListVm.LoadData();

                //if (clientListVm.ObservableClients is null)
                //{
                //    Logger.WriteLineToToastForced("No clients found.", Category.Warning);
                //    return;
                //}

                var loadedClients = new StringBuilder();
                foreach (var client in clientListVm.ObservableClients)
                {
                    var name = string.Empty;

                    if (client.DisplayName is null)
                    {
                        name = "DisplayName is null";
                    }
                    else
                    {
                        name = client.DisplayName;
                    }
                    //var name   = client?.DisplayName ?? "DisplayName is null";
                    var number = client?.MainNumber ?? "MainNumber is null";

                    loadedClients.AppendLine($"{name} {number}");
                }

                Logger.WriteLine(loadedClients.ToString(), Category.Information);

                ListView.ItemsSource = clientListVm.ObservableClients;
            }
            catch (Exception e)
            {
                Logger.WriteLine("Problem loading client/contact data", Category.Error, e);
            }

        }

        private async void AddToolbarItem_OnClicked(object sender
                                                  , EventArgs e)
        {
            // var progress = new Progress<string>(message => { BackUpDbButton.Text = message; });
            //
            // success = await Task.Run(() => BackupDbWork(progress))
            //                     .ConfigureAwait(false);
            //

            try
            {
                var clientAdded = clientListVm.AddNewClient();

                Logger.WriteLineToToastForced($"Client was added: {clientAdded}", Category.Information);

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

        private void SearchToolbarItem_OnClicked(object sender
                                               , EventArgs e)
        {
            Filter.IsVisible = !Filter.IsVisible;
        }

        private void Filter_OnTextChanged(object sender
                                        , TextChangedEventArgs e)
        {
            ListView.ItemsSource = clientListVm.SearchClients(Filter.Text);
        }

        private void OnSelectionChanged(object sender
                                      , ItemSelectionChangedEventArgs e)
        {
            var item = (Client)e.AddedItems?.FirstOrDefault();

            if (item == null) { return; }

            ListView.SelectedItems.Clear();

            PageNavigation.NavigateTo(nameof(ClientEditPage)
                                    , nameof(ClientEditPage.ClientId)
                                    , item.ClientId.ToString());
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
            var itemDeleted = clientListVm.Delete(SwipedItem);

            if (!itemDeleted.success)
            {
                Logger.WriteLine("Client could not be deleted.  Please try again."
                               , Category.Warning);
            }

            ListView.ItemsSource = clientListVm.ObservableClients;

            Logger.WriteLine($"Deleted Client: {itemDeleted.item} deleted."
                           , Category.Information);

            ListView.ResetSwipe();
        }

        private void SyncContactsToolbarItem_OnClicked(object sender
                                                     , EventArgs e)
        {
            clientListVm.SyncContactsToAppDatabase();
        }
    }
}