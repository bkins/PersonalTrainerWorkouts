using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;

using Avails.D_Flat.Extensions;
using Avails.Xamarin;
using Avails.Xamarin.Logger;

using PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;
using PersonalTrainerWorkouts.ViewModels.HelperClasses;
using PersonalTrainerWorkouts.ViewModels.Tab_Clients;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Exception = System.Exception;
using Logger = Avails.Xamarin.Logger.Logger;
using StringBuilder = System.Text.StringBuilder;

namespace PersonalTrainerWorkouts.Views.Tab_Clients;

[QueryProperty(nameof(ClientId)
             , nameof(ClientId))]
[QueryProperty(nameof(GoalId)
            , nameof(GoalId))]
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class GoalsAddEditPage : ContentPage, IQueryAttributable
{
    // ReSharper disable InconsistentNaming
    private string _clientId { get; set; }
    public string ClientId
    {
        get => _clientId;
        set => LoadClient(value);
    }

    private string _goalId { get; set; }
    public string GoalId
    {
        get => _goalId;
        set => LoadGoal(value);
    }

    public bool            IsNewGoal { get; set; }
    //public Goal            Goal      { get; set; }
    public GoalViewModel        GoalVm       { get; set; }
    public ClientViewModel      ClientVm     { get; set; }
    public MeasurablesViewModel MeasurableVm { get; set; }

    private void LoadGoal(string value)
    {
        _goalId   = value;
        IsNewGoal = _goalId == "0";
        Title     = IsNewGoal ? "New Goal" : "Edit Goal";

        if ( ! DataIsValidOrNewGoal()) return;

        var goal = ClientVm.Client
                           .Goals
                           .FirstOrDefault(goal => goal.Id == _goalId.ToSafeInt());

        GoalVm         = new GoalViewModel(goal);
        //MeasurableVm   = new MeasurablesViewModel(_goalId.ToSafeInt(), 0);
        BindingContext                       = GoalVm;
        //MeasurableCollectionView.ItemsSource = GoalVm.MeasurablesViewModelGroupByVariable;
        MeasurableCollectionView.ItemsSource = GoalVm.MeasurablesVmList
                                                     .Where(measurable => measurable.GoalSuccession != Enums.Succession.Baseline)
                                                     .OrderBy(measurable => measurable.Variable)
                                                     .ThenByDescending(measurable => measurable.GoalSuccession)
                                                     .ThenBy(measurable => measurable.DateTaken);
                                                    // .GroupBy(measurable => measurable.Variable);
    }

    private bool DataIsValidOrNewGoal()
    {
        if (ClientVm?.Client is not null) //   If the client VM was loaded successfully and has a client
            return ! IsNewGoal;           // , return whether or not this is a new goal

        Logger.WriteLine("Could not retrieve the Client.  Something went wrong."
                       , Category.Error);

        return false;
    }

    public GoalsAddEditPage()
    {
        InitializeComponent();
        GoalVm = new GoalViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        LogWhenMultipleStatusesAreTrue();
    }

    private void LogWhenMultipleStatusesAreTrue()
    {
        if (GoalVm?.Goal is null) return;
        
        GoalVm.Goal.GetStatus();
        
        var statuses = new List<bool>()
                           {
                               GoalVm.Goal.Failed
                             , GoalVm.Goal.InProcess
                             , GoalVm.Goal.MissedTarget
                             , GoalVm.Goal.NotStarted
                             , GoalVm.Goal.SuccessfullyCompleted
                           };
        if (statuses.Count(status => status) > 1)
        {
            var extraDetails = new StringBuilder();
            extraDetails.Append(Environment.NewLine);
            extraDetails.Append($"{nameof(GoalVm.Goal.DateStarted)}: ");
            extraDetails.AppendLine(GoalVm.Goal.DateStarted.HasValue 
                                        ? GoalVm.Goal.DateStarted.Value.ToString("d") 
                                        : "null");

            extraDetails.Append($"{nameof(GoalVm.Goal.TargetDate)}: ");
            extraDetails.AppendLine(GoalVm.Goal.TargetDate.ToString("d"));

            extraDetails.Append($"{nameof(GoalVm.Goal.DateCompleted)}: ");
            extraDetails.AppendLine(GoalVm.Goal.DateCompleted.HasValue 
                                        ? GoalVm.Goal.DateCompleted.Value.ToString("d") 
                                        : "null");

            extraDetails.Append($"{nameof(GoalVm.Goal.GetStatus)}: ");
            extraDetails.AppendLine(GoalVm.Goal.GetStatus().GetDescription());

            extraDetails.Append($"{nameof(GoalVm.Goal.Failed)}: ");
            extraDetails.AppendLine(GoalVm.Goal.Failed.ToString());

            extraDetails.Append($"{nameof(GoalVm.Goal.InProcess)}: ");
            extraDetails.AppendLine(GoalVm.Goal.InProcess.ToString());

            extraDetails.Append($"{nameof(GoalVm.Goal.MissedTarget)}: ");
            extraDetails.AppendLine(GoalVm.Goal.MissedTarget.ToString());

            extraDetails.Append($"{nameof(GoalVm.Goal.NotStarted)}: ");
            extraDetails.AppendLine(GoalVm.Goal.NotStarted.ToString());

            extraDetails.Append($"{nameof(GoalVm.Goal.SuccessfullyCompleted)}: ");
            extraDetails.AppendLine(GoalVm.Goal.SuccessfullyCompleted.ToString());

            Logger.WriteLineToToastForced("More than one status is set!"
                                        , Category.Warning
                                        , extraDetails.ToString()
                                        , null);
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, string> query)
    {
        try
        {
            // The query parameter requires URL decoding.
            ClientId = HttpUtility.UrlDecode(query[nameof(ClientId)]);
            GoalId   = HttpUtility.UrlDecode(query[nameof(GoalId)]);

            SetPageControls();
        }
        catch (Exception e)
        {
            Logger.WriteLine($"Failed initiate {nameof(GoalsAddEditPage)}."
                           , Category.Error
                           , e);
            throw;
        }
    }

    private void LoadClient(string clientId)
    {
        _clientId = clientId;
        ClientVm  = new ClientViewModel(clientId);
    }

    private void SetPageControls()
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            if (IsNewGoal)
            {
                //set any default values for a new Goal
                //TargetDatePicker.Date = DateTime.Today.AddMonths(1);
            
                return;
            }
            
            //Editing an existing goal. Set the Goal's properties to the page's controls 
            NameEditor.Text        = GoalVm.Goal.Name;
            DescriptionEditor.Text = GoalVm.Goal.Description;

            // if (Goal.DateStarted != null) StartDatePicker.Date = Goal.DateStarted.Value;
            // if (Goal.DateCompleted != null) CompletedDatePicker.Date = Goal.DateCompleted.Value;

            // if (GoalVm.Goal.DateStarted != null)   StartDatePicker.NullableDate     = GoalVm.Goal.DateStarted;
            // if (GoalVm.Goal.DateCompleted != null) CompletedDatePicker.Date = GoalVm.Goal.DateCompleted.Value;

            // TargetDatePicker.Date = GoalVm.Goal.TargetDate;
            
            //SuccessfullyCompletedCheckbox.IsChecked = Goal.SuccessfullyCompleted;
            FailedCheckbox.IsChecked = GoalVm.Goal.Failed;
            
            CalculateStatus();
        });
    }

    private void CalculateStatus()
    {
        if (GoalVm?.Goal is null) return;
        
        GoalVm.Goal?.CalculateStatus();
        
        var status = GoalVm?.Goal?.GetStatus();
        //StatusEditor.Text = status.GetDescription();
        
        GoalVm?.CalculateStatus();
    }
    
    private void SaveButton_OnClicked(object sender
                                    , EventArgs e)
    {
        if (IsNewGoal) SetNewGoal();
        else           SetGoalFromPageFields();

        ClientVm.Save(GoalVm.Goal);
        PageNavigation.NavigateBackwards();
    }

    private void SetGoalFromPageFields()
    {
        GoalVm.Goal.Name          = NameEditor.Text;
        GoalVm.Goal.Description   = DescriptionEditor.Text;
        GoalVm.Goal.DateStarted   = StartDatePicker.NullableDate;
        GoalVm.Goal.TargetDate    = TargetDatePicker.Date;
        GoalVm.Goal.DateCompleted = CompletedDatePicker.NullableDate;
        GoalVm.Goal.Failed        = FailedCheckbox.IsChecked;
    }

    private void SetNewGoal()
    {
        GoalVm.Goal               ??= new Goal();
        GoalVm.Goal.ClientId      =   ClientVm.Client.Id;
        GoalVm.Goal.Name          =   NameEditor.Text;
        GoalVm.Goal.Description   =   DescriptionEditor.Text;
        GoalVm.Goal.DateStarted   =   StartDatePicker.NullableDate;
        GoalVm.Goal.TargetDate    =   TargetDatePicker.Date;
        GoalVm.Goal.DateCompleted =   CompletedDatePicker.NullableDate;
        GoalVm.Goal.Failed        =   FailedCheckbox.IsChecked;
    }

    private void CompletedDatePicker_OnDateSelected(object               sender
                                                  , DateChangedEventArgs e)
    {
        //CalculateStatus();
    }

    private void TargetDatePicker_OnDateSelected(object               sender
                                               , DateChangedEventArgs e)
    {
        //SetStatusControls();
    }

    private void StartDatePicker_OnDateSelected(object               sender
                                              , DateChangedEventArgs e)
    {
        //SetStatusControls();
    }
    
    private void SetStatusControls()
    {
        var goalState = ValidateGoal();
        
        if (goalState.Invalid) return;

        // Device.BeginInvokeOnMainThread(() =>
        // {
        //     // StatusImage.Source = goalState.Status switch
        //     // {
        //     //     Status.CompletedSuccessfully => GetImageSource("Success_48.png")
        //     //   , Status.NotStarted            => GetImageSource("NotStarted_48.png")
        //     //   , Status.InProgress            => GetImageSource("InProgress_48.png")
        //     //   , Status.MissTarget            => GetImageSource("MissedTarget_48.png")
        //     //   , Status.Failed                => GetImageSource("Failed_48.png")
        //     //   , Status.Unknown               => GetImageSource("Unknown_48.png")
        //     //   , _ => throw new ArgumentOutOfRangeException()
        //     // };
        //
        //     //StatusEditor.Text = goalState.Status.GetDescription();
        // });
    }

    private (bool Invalid, Goal.Status Status) ValidateGoal()
    {
        var goalInvalidStatusUnknown = (true, Goal.Status.Unknown);
        
        if (GoalVm.Goal is null) return goalInvalidStatusUnknown;

        var status = GoalVm.Goal.GetStatus();

        if (status is Goal.Status.Unknown) LogIt();
        
        var goalValidWithStatus = (false, status);
        
        return goalValidWithStatus;
    }

    private void LogIt()
    {
        var extraDetails = BuildExtraDetails();

        Logger.WriteLine($"A Goal Status of {nameof(Goal.Status.Unknown)} was returned. This should never happen"
                       , Category.Warning
                       , null
                       , extraDetails.ToString());
    }

    private StringBuilder BuildExtraDetails()
    {
        var extraDetails = new StringBuilder();
        extraDetails.Append("StartDate: ");
        extraDetails.Append(GoalVm.Goal.DateStarted.HasValue 
                                ? GoalVm.Goal.DateStarted.ToString() 
                                : "null");
        extraDetails.Append(Environment.NewLine);
        extraDetails.Append("TargetDate: ");
        extraDetails.Append(GoalVm.Goal.TargetDate.ToString(CultureInfo.CurrentCulture));
        extraDetails.Append(Environment.NewLine);
        extraDetails.Append("DateCompleted: ");
        extraDetails.Append(GoalVm.Goal.DateCompleted.ToString());
        extraDetails.Append(Environment.NewLine);
        extraDetails.Append("Failed?: ");
        extraDetails.Append(GoalVm.Goal.Failed);
        
        return extraDetails;
    }
    
    private ImageSource GetImageSource(string fileName)
    {
        return Device.RuntimePlatform == Device.Android
                            ? ImageSource.FromResource(fileName)
                            : ImageSource.FromFile($"Images/{fileName}");
    }

    private void CompletedDatePicker_OnPropertyChanged(object                   sender
                                                      , PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(CompletedDatePicker.NullableDate)) return;
        
        if (GoalVm is not null)
        {
            GoalVm.Goal.DateCompleted = CompletedDatePicker.NullableDate;
        }
        CalculateStatus();
    }

    private void LogGoalVmData_OnClicked(object    sender
                                       , EventArgs e)
    {
        Logger.WriteLine("GoalVm Data", Category.Information, null, GoalVm.ToString());
        Logger.WriteLineToToastForced("VM Data Logged", Category.Information);
    }

    private void MeasurablesCollectionView_OnSelectionChanged(object                    sender
                                                            , SelectionChangedEventArgs e)
    {

    }

    private void AddNewMeasurableButton_OnClicked(object    sender
                                                , EventArgs e)
    {
        PageNavigation.NavigateTo(nameof(MeasurablesAddPage)
                                , nameof(MeasurablesAddPage.ClientId)
                                , ClientId
                                , nameof(MeasurablesAddPage.GoalId)
                                , GoalId);
    }

    private void MeasurableCollectionView_OnSelectionChanged(object                    sender
                                                           , SelectionChangedEventArgs e)
    {

    }
}
