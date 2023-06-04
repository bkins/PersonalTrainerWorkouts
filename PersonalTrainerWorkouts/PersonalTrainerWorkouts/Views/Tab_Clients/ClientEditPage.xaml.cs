using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.ViewModels;
using PersonalTrainerWorkouts.ViewModels.Tab_Clients;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;

namespace PersonalTrainerWorkouts.Views.Tab_Clients
{
    [QueryProperty(nameof(ClientId)
                 , nameof(ClientId))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientEditPage : IQueryAttributable
    {
        public string          ClientId { get; set; }
        public ClientViewModel ClientVm { get; set; }
        public GoalViewModel   GoalVm   { get; set; }

        public ClientEditPage()
        {
            InitializeComponent();

            //ClientVm = new ClientViewModel();
        }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            try
            {
                ClientId = HttpUtility.UrlDecode(query[nameof(ClientId)]);
                ClientVm = new ClientViewModel(ClientId);

                LoadData();
            }
            catch (Exception e)
            {
                Logger.WriteLine($"Failed to initiate {nameof(ClientEditPage)}."
                               , Category.Error
                               , e);
            }
        }

        private void LoadData()
        {
            ClientNameEntry.Text           = ClientVm.Client.DisplayName;
            ClientMainPhoneValueLabel.Text = ClientVm.Client.MainNumber;
            ClientNoteRte.Text             = ClientVm.Client.Notes;

            //Task.Run(() =>
            //         {
            //             var goals = ClientVm.Client.Goals;

            //             var goal = goals?.FirstOrDefault();
            //             GoalVm = new GoalViewModel(goal);

            //         }).ConfigureAwait(false);

            //BindingContext = ClientVm;
            
            GoalsCollectionView.ItemsSource = ClientVm.Goals;
            PhoneNumberPicker.ItemsSource = ClientVm.PhoneNumbers;
            
            Title = ClientVm.Client.DisplayName;

            //var goals = ClientVm.Client.Goals;

            //var firstGoalName = goals?.FirstOrDefault()
            //                         ?.Name;
        }

        #region Events

        private void SaveButton_OnClicked(object    sender
                                        , EventArgs e)
        {
            //ViewModel.Client.DisplayName = ClientNameEntry.Text;
            var displayName = ClientVm.Client.ToString();

            //TODO: Update properties unique to the Client class, not in Contact
            ClientVm.Client.Notes = ClientNoteRte.HtmlText;
            try
            {
                ClientVm.Save();
            }
            catch (Exception exception)
            {
                Logger.WriteLine("Client could not be saved"
                               , Category.Error
                               , exception
                               , $"Client: {displayName}");
            }

            PageNavigation.NavigateBackwards();
        }

        private void LinkContactToolbarItem_OnClicked(object    sender
                                                    , EventArgs e)
        {
            ClientVm.LinkContactToClient().ConfigureAwait(false);
        }

        private void ClientMainPhoneValueLabel_OnTapped(object    sender
                                                      , EventArgs e)
        {
            try
            {
                PhoneDialer.Open(ClientMainPhoneValueLabel.Text);
            }
            catch (FeatureNotSupportedException ex)
            {
                Logger.WriteLineToToastForced("This feature is not supported on your phone"
                                            , Category.Information
                                            , ex);
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Problem while trying to dial number. "
                               , Category.Error
                               , ex
                               , ClientVm.Client.DisplayName);
            }
        }

        private void ClientMainPhoneLabel_OnTapped(object    sender
                                                 , EventArgs e)
        {
            
        }

        private void PhoneNumberPicker_OnOkButtonClicked(object                    sender
                                                       , SelectionChangedEventArgs e)
        {
            PhoneNumberPicker.IsVisible = false;

            var selectedNumber = PhoneNumberPicker.SelectedItem as PhoneNumber;
            
            ClientVm.SetNewMainNumber(selectedNumber);
            ClientMainPhoneValueLabel.Text = selectedNumber?.Number ?? "<main number not set>";
        }

        private void PhoneNumberPicker_OnCancelButtonClicked(object                    sender
                                                           , SelectionChangedEventArgs e)
        {
            PhoneNumberPicker.IsVisible = false;
        }

        private void AddNewGoalButton_OnClicked(object    sender
                                              , EventArgs e)
        {
            PageNavigation.NavigateTo(nameof(GoalsAddEditPage)
                                    , nameof(GoalsAddEditPage.ClientId)
                                    , ClientId
                                    , nameof(GoalsAddEditPage.GoalId)
                                    , "0");
        }

        private void GoalsCollectionView_OnSelectionChanged(object                                  sender
                                                          , Xamarin.Forms.SelectionChangedEventArgs e)
        {
            var goal = (GoalViewModel)e.CurrentSelection.FirstOrDefault();

            if (goal == null) return;
            
            PageNavigation.NavigateTo(nameof(GoalsAddEditPage)
                                    , nameof(GoalsAddEditPage.ClientId)
                                    , ClientId
                                    , nameof(GoalsAddEditPage.GoalId)
                                    , goal.Goal.Id.ToString());
        }

        private void ChangeMainNumberButton_OnClicked(object    sender
                                                    , EventArgs e)
        {
            //Bring up picker of a list of number that are in the Client to reassign the main number
            PhoneNumberPicker.IsVisible = true;
        }

        #endregion

        
    }
}