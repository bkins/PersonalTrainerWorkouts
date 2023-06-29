using System;
using System.Collections.Generic;
using System.Web;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Avails.D_Flat.Extensions;
using Avails.Xamarin;
using Avails.Xamarin.Logger;

using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.ViewModels.Tab_Sessions;

using Syncfusion.ListView.XForms;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;
using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;

namespace PersonalTrainerWorkouts.Views.Tab_Sessions
{
    [QueryProperty(nameof(SessionId)
                 , nameof(SessionId))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionEditPage : IQueryAttributable
    {
        public string           SessionId { get; set; }
        public SessionViewModel ViewModel { get; set; }

        private Client _selectedClient;

        public SessionEditPage()
        {
            InitializeComponent();

            ViewModel = new SessionViewModel();
        }
        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            try
            {
                SessionId = HttpUtility.UrlDecode(query[nameof(SessionId)]);
                ViewModel = new SessionViewModel(SessionId);

                LoadData();
            }
            catch (Exception e)
            {
                Logger.WriteLine($"Failed initiate {nameof(SessionEditPage)}."
                               , Category.Error
                               , e);
            }
        }

        private void LoadData()
        {
            SessionStartDateEntry.Text = ViewModel.NewSession.StartDate.ToShortDateTimeString();
            SessionEndDateEntry.Text   = ViewModel.NewSession.EndDate.ToShortDateTimeString();
            SessionNotesRtEditor.Text  = ViewModel.NewSession.Note;

            ClientPickerLabel.Text     = ViewModel.NewSession
                                                  .Client
                                                  ?.DisplayName
                                        ?? "Click here to select a client";

            ClientPicker.ItemsSource = ViewModel.ClientListViewModel.Clients;
        }

        private void SaveButton_OnClicked(object    sender
                                        , EventArgs e)
        {
            ViewModel.NewSession.StartDate = GetParseDateTime(StartDatePicker.Date
                                                            , StartTimePicker.Time);
            ViewModel.NewSession.EndDate = GetParseDateTime(EndDatePicker.Date
                                                          , EndTimePicker.Time);
            ViewModel.NewSession.Note = SessionNotesRtEditor.Text;

            if (ViewModel.SaveSession())
            {
                PageNavigation.NavigateBackwards();
            }
        }

        private static DateTime GetParseDateTime(DateTime date
                                               , TimeSpan time)
        {
            var dateTime = $"{date.ToShortDateString()} {time.ToString()}";

            return DateTime.Parse(dateTime);
        }

        private void SelectClient_OnTapped(object    sender
                                         , EventArgs e)
        {
            ClientPicker.IsVisible = true;
        }

        private void ClientPicker_OnOkButtonClicked(object                    sender
                                                  , SelectionChangedEventArgs e)
        {
            ViewModel.NewSession.Client = (Client)ClientPicker.SelectedItem;
            ClientPickerLabel.Text      = ViewModel.NewSession.Client.DisplayName;
            ClientPicker.IsVisible      = false;
        }

        private void ClientPicker_OnCancelButtonClicked(object                    sender
                                                      , SelectionChangedEventArgs e)
        {
            ClientPicker.IsVisible = false;
        }

        private void SelectWorkout_OnTapped(object    sender
                                          , EventArgs e)
        {

        }

        private void WorkoutsPicker_OnOkButtonClicked(object                    sender
                                                    , SelectionChangedEventArgs e)
        {

        }

        private void WorkoutsPicker_OnCancelButtonClicked(object                    sender
                                                        , SelectionChangedEventArgs e)
        {

        }

        private void AddRemoveButton_OnClicked(object    sender
                                             , EventArgs e)
        {

        }

        private void LeftImage_BindingContextChanged(object    sender
                                                   , EventArgs e)
        {

        }

        private void OnSelectionChanged(object                        sender
                                      , ItemSelectionChangedEventArgs e)
        {

        }

        private void ListView_SwipeEnded(object              sender
                                       , SwipeEndedEventArgs e)
        {

        }
    }
}
