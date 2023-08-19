using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.ViewModels.Tab_Sessions;
using PersonalTrainerWorkouts.Views.Tab_Clients;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;

namespace PersonalTrainerWorkouts.Views.Tab_Sessions;

[QueryProperty(nameof(SessionId)
             , nameof(SessionId))]
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class NewSessionEditPage: IQueryAttributable, INotifyPropertyChanged
{
    public string           SessionId { get; set; }
    public SessionViewModel ViewModel { get; set; }

    private Client        _selectedClient;
    private List<Workout> _selectedWorkouts;

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
    public event PropertyChangedEventHandler PropertyChanged;

    public NewSessionEditPage()
    {
        InitializeComponent();

        ViewModel         = new SessionViewModel();
        _selectedWorkouts = new List<Workout>();
    }

    public NewSessionEditPage(string sessionId)
    {
        SessionId = sessionId;
    }

    protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                              ?? "Tap here to select a client";

        var startDate = ViewModel.NewSession.StartDate;
        var endDate   = ViewModel.NewSession.EndDate;
        var minValue  = DateTime.MinValue;
        var now       = DateTime.Now;

        StartDatePicker.Date         = startDate == minValue ? now : startDate;
        EndDatePicker.Date           = endDate   == minValue ? now : endDate;
        StartTimePicker.Time         = startDate == minValue ? now.TimeOfDay : startDate.TimeOfDay;
        EndTimePicker.Time           = endDate   == minValue ? now.TimeOfDay : endDate.TimeOfDay;
        SessionNotesRtEditor.Text    = ViewModel.NewSession.Note;
        WorkoutComboBox.SelectedItem = ViewModel.SelectedWorkouts;
        ClientPicker.ItemsSource     = ViewModel.ClientListViewModel.Clients;
    }

    private void ToggleClientPickersVisibility(bool currentVisibility)
    {
        ClientPicker.IsVisible         = ! currentVisibility;
        StartDateLabel.IsVisible       = ! ClientPicker.IsVisible;
        StartDatePicker.IsVisible      = ! ClientPicker.IsVisible;
        StartTimePicker.IsVisible      = ! ClientPicker.IsVisible;
        EndDateLabel.IsVisible         = ! ClientPicker.IsVisible;
        EndDatePicker.IsVisible        = ! ClientPicker.IsVisible;
        EndTimePicker.IsVisible        = ! ClientPicker.IsVisible;
        WorkoutComboBox.IsVisible      = ! ClientPicker.IsVisible;
        SessionNotesRtEditor.IsVisible = ! ClientPicker.IsVisible;
    }

    private void ClientPicker_OnCancelButtonClicked(object                    sender
                                                  , SelectionChangedEventArgs e)
    {
        ToggleClientPickersVisibility(ClientPicker.IsVisible);
    }

    private void ClientPicker_OnOkButtonClicked(object                    sender
                                              , SelectionChangedEventArgs e)
    {
        ViewModel.NewSession.Client = (Client)ClientPicker.SelectedItem;
        ClientPickerLabel.Text      = ViewModel.NewSession.Client.DisplayName;

        ToggleClientPickersVisibility(ClientPicker.IsVisible);
    }

    private void WorkoutsPicker_OnCancelButtonClicked(object                    sender
                                                    , SelectionChangedEventArgs e)
    {

    }

    private void WorkoutComboBox_OnSelectionChanged(object                                               sender
                                                  , Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
    {
        InitiateViewModelSelectedWorkouts();

        var selectedWorkouts = (List<object>) e.Value;
        foreach (var workout in selectedWorkouts)
        {
            ViewModel.SelectedWorkouts.Add((Workout)workout);
        }

        var test = ViewModel.SelectedWorkouts.Count;
    }

    private void InitiateViewModelSelectedWorkouts()
    {
        if (ViewModel.SelectedWorkouts is null)
        {
            ViewModel.SelectedWorkouts = new ObservableCollection<Workout>();
        }
        else
        {
            ViewModel.SelectedWorkouts.Clear();
        }
    }

    private void WorkoutsPicker_OnOkButtonClicked(object                    sender
                                                , SelectionChangedEventArgs e)
    {

    }

    private void SelectClient_OnTapped(object obj)
    {
        ToggleClientPickersVisibility(ClientPicker.IsVisible);
    }

    private void SaveButton_OnClicked(object    sender
                                    , EventArgs e)
    {
        ViewModel.NewSession.StartDate = GetParseDateTime(StartDatePicker.Date
                                                        , StartTimePicker.Time);
        ViewModel.NewSession.EndDate = GetParseDateTime(EndDatePicker.Date
                                                      , EndTimePicker.Time);
        ViewModel.NewSession.Note = SessionNotesRtEditor.Text;

        var newListOfWorkouts = ((IEnumerable)WorkoutComboBox.SelectedItem)?.Cast<Workout>().ToList();

        ViewModel.NewSession.Workouts = newListOfWorkouts;

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

    private void AddRemoveButton_OnClicked(object    sender
                                         , EventArgs e)
    {

    }

    private void EditClientImageButton_Tapped(object obj)
    {
        if (ViewModel?.NewSession?.Client is null)
        {
            Logger.WriteLineToToastForced("You must select a client first."
                                        , Category.Warning);
            return;
        }

        PageNavigation.NavigateTo(new ClientEditPage(ViewModel.NewSession.Client.Id.ToString()));
    }

    private void StartDatePickerOnDateSelected(object               sender
                                             , DateChangedEventArgs e)
    {
        EndDatePicker.Date = StartDatePicker.Date;
    }
}
