using Avails.Xamarin;
using Avails.Xamarin.Logger;

using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.ViewModels;

using Syncfusion.ListView.XForms;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;
using SwipeStartedEventArgs = Syncfusion.ListView.XForms.SwipeStartedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientListPage
    {
        public int SwipedItem { get; set; }
        public ClientListViewModel ClientListVm { get; set; }

        public ClientListPage()
        {
            InitializeComponent();
            ClientListVm = new ClientListViewModel();
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

                ClientListVm?.LoadData();

                //if (ClientListVm?.ObservableClients is null) 

                ListView.ItemsSource = ClientListVm?.ObservableClients;

                //var loadedClients = new StringBuilder();
                //foreach (var client in ClientListVm.ObservableClients)
                //{
                //    var name = string.Empty;

                //    name = client.DisplayName ?? "DisplayName is null";

                //    //var name   = client?.DisplayName ?? "DisplayName is null";
                //    var number = client.MainNumber ?? "MainNumber is null";

                //    loadedClients.AppendLine($"{name} {number}");
                //}

                //Logger.WriteLine(loadedClients.ToString(), Category.Information);

            }
            catch (Exception e)
            {
                Logger.WriteLine("Problem loading client/contact data: ", Category.Error, e);
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
                var clientAdded = await ClientListVm.AddNewClient();

                Logger.WriteLineToToastForced($"Client was added: {clientAdded}", Category.Information);


                ListView.ItemsSource = ClientListVm.ObservableClients;

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
            ListView.ItemsSource = ClientListVm.SearchClients(Filter.Text);
        }

        private void OnSelectionChanged(object sender
                                      , ItemSelectionChangedEventArgs e)
        {
            var item = (Client)e.AddedItems?.FirstOrDefault();

            if (item == null) { return; }

            ListView.SelectedItems.Clear();

            PageNavigation.NavigateTo(nameof(ClientEditPage)
                                    , nameof(ClientEditPage.ClientId)
                                    , item.Id.ToString());
        }

        private void ListView_OnSwipeStarted(object sender
                                           , SwipeStartedEventArgs e)
        {
            SwipedItem = -1;
        }

        private void ListView_SwipeEnded(object sender
                                       , SwipeEndedEventArgs e)
        {
            SwipedItem = e.ItemIndex;
        }

        private void Delete()
        {
            var itemDeleted = ClientListVm.Delete(SwipedItem);
            var client = ClientListVm.ObservableClients[SwipedItem];

            DeleteClient(itemDeleted, client);

            ListView.ItemsSource = ClientListVm.ObservableClients;

            ListView.ResetSwipe();
        }

        private void DeleteClient((string item, bool success, DeleteReasons reason) itemDeleted
                                , Client client)
        {
            if (itemDeleted is { success: false, reason: DeleteReasons.HasSessions })
            {
                var task = DisplayAlert("Client has sessions."
                                        , $"You cannot delete this client ({client.DisplayName}) because it has sessions.  Would you like to delete all the sessions for this client and then delete the client record?"
                                        , "Yes"
                                        , "No");

                task.ContinueWith(action =>
                                  {
                                      itemDeleted = ContinuationAction(action
                                                                     , itemDeleted
                                                                     , client);
                                  });
            }

            if (!itemDeleted.success)
            {
                Logger.WriteLine("Client could not be deleted.  Please try again."
                               , Category.Warning);
            }

        }

        private (string item
               , bool success
               , DeleteReasons reason)
                ContinuationAction(Task<bool> action
                                 , (string item
                                  , bool success
                                  , DeleteReasons reason) itemDeleted
                                 , Client client)
        {
            switch (action.Status)
            {
                case TaskStatus.RanToCompletion when action.Result:

                    itemDeleted = DeleteClientSessions(client);

                    Logger.WriteLineToToastForced(itemDeleted.item
                                                , Category.Information);

                    itemDeleted = ClientListVm.Delete(SwipedItem);

                    Logger.WriteLineToToastForced(itemDeleted.item
                                                , Category.Information);

                    break;

                case TaskStatus.Faulted:

                    Logger.WriteLineToToastForced(itemDeleted.item
                                                , Category.Information);

                    break;

                case TaskStatus.Canceled:

                    break;

                case TaskStatus.Created:

                    break;

                case TaskStatus.Running:

                    break;

                case TaskStatus.WaitingForActivation:

                    break;

                case TaskStatus.WaitingForChildrenToComplete:

                    break;

                case TaskStatus.WaitingToRun:

                    break;

                default:

                    throw new ArgumentOutOfRangeException();
            }

            return itemDeleted;
        }

        private (string item, bool success, DeleteReasons reason) DeleteClientSessions(Client client)
        {
            var itemDeleted = ClientListVm.DeleteClientSessions(SwipedItem);

            if (itemDeleted.success)
            {
                Logger.WriteLineToToastForced($"All sessions for {client.DisplayName} were deleted."
                                            , Category.Information);
            }

            itemDeleted = ClientListVm.Delete(SwipedItem);

            return itemDeleted;
        }

        private void SyncContactsToolbarItem_OnClicked(object sender
                                                     , EventArgs e)
        {
            ClientListVm.SyncContactsToAppDatabase();
        }

        private void DialNumber()
        {
            var itemToCall = ClientListVm.ObservableClients[SwipedItem];
            PhoneDialer.Open(itemToCall.MainNumber);

            ListView.ResetSwipe();
        }

        private void SendSms()
        {
            var itemToText = ClientListVm.ObservableClients[SwipedItem];
            Sms.ComposeAsync(new SmsMessage(string.Empty, itemToText.MainNumber));

            ListView.ResetSwipe();
        }

        private void SendTextImage_OnBindingContextChanged(object sender
                                                     , EventArgs e)
        {
            if (sender is not Image image)
                return;

            var imageParent = image.Parent as View;

            imageParent?.GestureRecognizers
                        .Add(new TapGestureRecognizer
                        {
                            Command = new Command(SendSms)
                        });
        }

        private void SendTextImage_OnTapped(object sender
                                          , EventArgs e)
        {
            SendSms();
        }

        private void DialNumberImage_OnBindingContextChanged(object sender
                                                           , EventArgs e)
        {
            if (sender is not Image image)
                return;

            var imageParent = image.Parent as View;

            imageParent?.GestureRecognizers
                        .Add(new TapGestureRecognizer
                        {
                            Command = new Command(DialNumber)
                        });
        }

        private void DialNumberImage_OnTapped(object sender
                                            , EventArgs e)
        {
            DialNumber();
        }

        private void DeleteImage_OnBindingContextChanged(object sender
                                                       , EventArgs e)
        {
            if (sender is not Image image)
                return;

            var imageParent = image.Parent as View;

            imageParent?.GestureRecognizers
                        .Add(new TapGestureRecognizer
                        {
                            Command = new Command(Delete)
                        });
        }

        private void DeleteImage_OnTapped(object sender
                                        , EventArgs e)
        {
            Delete();
        }
    }
}