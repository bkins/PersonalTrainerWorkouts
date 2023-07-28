using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Web;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.ViewModels.Tab_Sessions;

using Syncfusion.ListView.XForms;
using Syncfusion.SfPicker.XForms;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;
using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;

namespace PersonalTrainerWorkouts.Views.Tab_Sessions
{
    [QueryProperty(nameof(SessionId)
                 , nameof(SessionId))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionEditPage : IQueryAttributable, INotifyPropertyChanged
    {
        public string           SessionId { get; set; }
        public SessionViewModel ViewModel { get; set; }

        private Client                           _selectedClient;
        private List<Workout>                    _selectedWorkouts;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private object selectedIndices;
        public object SelectedIndices
        {
            get => selectedIndices;
            set
            {
                selectedIndices = value;
                NotifyPropertyChanged();
            }
        }

        public SessionEditPage()
        {
            InitializeComponent();

            ViewModel         = new SessionViewModel();
            _selectedWorkouts = new List<Workout>();
        }

        public SessionEditPage(string sessionId)
        {
            SessionId = sessionId;
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
            ClientPickerLabel.Text = ViewModel.NewSession
                                              .Client
                                              ?.DisplayName
                                  ?? "Click here to select a client";

            var startDate = ViewModel.NewSession.StartDate;
            var endDate   = ViewModel.NewSession.EndDate;
            var minValue  = DateTime.MinValue;
            var now       = DateTime.Now;

            StartDatePicker.Date = startDate == minValue ? now : startDate;
            EndDatePicker.Date   = endDate   == minValue ? now : endDate;
            StartTimePicker.Time = startDate == minValue ? now.TimeOfDay : startDate.TimeOfDay;
            EndTimePicker.Time   = endDate   == minValue ? now.TimeOfDay : endDate.TimeOfDay;

            // SessionStartDateEntry.Text = ViewModel.NewSession.StartDate.ToShortDateTimeString();
            // SessionEndDateEntry.Text   = ViewModel.NewSession.EndDate.ToShortDateTimeString();
            SessionNotesRtEditor.Text = ViewModel.NewSession.Note;



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

            ViewModel.NewSession.Workouts = WorkoutComboBox.SelectedItem as List<Workout>;

            if (ViewModel.SaveSession())
            {
                //BENDO: investigate why:
                //at this point ViewModel.NewSession.Client is null
                //it was not null before this point
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
            ToggleClientPickersVisibility(ClientPicker.IsVisible);
        }

        private void ToggleClientPickersVisibility(bool currentVisibility)
        {

            ClientPicker.IsVisible          = ! currentVisibility;
            ClientLabel.IsVisible           = ! ClientPicker.IsVisible;
            SessionStartDateLabel.IsVisible = ! ClientPicker.IsVisible;
            StartDatePicker.IsVisible       = ! ClientPicker.IsVisible;
            StartTimePicker.IsVisible       = ! ClientPicker.IsVisible;
            SessionEndDateLabel.IsVisible   = ! ClientPicker.IsVisible;
            EndDatePicker.IsVisible         = ! ClientPicker.IsVisible;
            EndTimePicker.IsVisible         = ! ClientPicker.IsVisible;
            WorkoutComboBox.IsVisible       = ! ClientPicker.IsVisible;
            SessionNotesRtEditor.IsVisible  = ! ClientPicker.IsVisible;
        }

        private void ClientPicker_OnOkButtonClicked(object                    sender
                                                  , SelectionChangedEventArgs e)
        {
            ViewModel.NewSession.Client = (Client)ClientPicker.SelectedItem;
            ClientPickerLabel.Text      = ViewModel.NewSession.Client.DisplayName;

            ToggleClientPickersVisibility(ClientPicker.IsVisible);
        }

        private void ClientPicker_OnCancelButtonClicked(object                    sender
                                                      , SelectionChangedEventArgs e)
        {
            ToggleClientPickersVisibility(ClientPicker.IsVisible);
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

        private void WorkoutComboBox_OnSelectionChanged(object                                               sender
                                                      , Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            ViewModel.SelectedWorkouts.Clear();

            var selectedWorkouts = (List<object>) e.Value;
            foreach (var workout in selectedWorkouts)
            {
                ViewModel.SelectedWorkouts.Add((Workout)workout);
            }

            var test = ViewModel.SelectedWorkouts.Count;
            // _selectedWorkouts.Clear();
            //
            // var selectedWorkouts = (List<object>) e.Value;
            // foreach (var workout in selectedWorkouts)
            // {
            //     _selectedWorkouts.Add((Workout)workout);
            // }
            //
            // var test = _selectedWorkouts.Count;
        }
    }
}
