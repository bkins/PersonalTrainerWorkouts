using Avails.D_Flat.Extensions;
using Avails.Xamarin;
using Avails.Xamarin.Logger;

using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Logger = Avails.Xamarin.Logger.Logger;

namespace PersonalTrainerWorkouts.Views;

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

    public bool IsNewGoal { get; set; }
    public Goal Goal { get; set; }

    private void LoadGoal(string value)
    {
        _goalId = value;
        IsNewGoal = _goalId == "0";

        if (!DataIsValidOrNewGoal()) return;

        Goal = ClientVm.Client.Goals.FirstOrDefault(goal => goal.Id == _goalId.ToSafeInt());
    }

    private bool DataIsValidOrNewGoal()
    {
        if (ClientVm?.Client is not null) //   If the client VM was loaded successfully and has a client
            return !IsNewGoal;           // , return whether or not this is a new goal

        Logger.WriteLine("Could not retrieve the Client.  Something went wrong."
                       , Category.Error);

        return false;
    }

    public ClientViewModel ClientVm { get; set; }

    public GoalsAddEditPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, string> query)
    {
        try
        {
            // The query parameter requires URL decoding.
            ClientId = HttpUtility.UrlDecode(query[nameof(ClientId)]);
            GoalId = HttpUtility.UrlDecode(query[nameof(GoalId)]);

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
        ClientVm = new ClientViewModel(clientId);
    }

    private void SetPageControls()
    {
        if (IsNewGoal)
        {
            TargetDatePicker.Date = DateTime.Today.AddMonths(1);
            return;
        }

        DescriptionEditor.Text = Goal.Description;

        if (Goal.DateStarted != null) { StartDatePicker.Date = Goal.DateStarted.Value; }

        TargetDatePicker.Date = Goal.TargetDate;
        SuccessfullyCompletedCheckbox.IsChecked = Goal.SuccessfullyCompleted;
        FailedCheckbox.IsChecked = Goal.Failed;
    }

    private void SaveButton_OnClicked(object sender
                                    , EventArgs e)
    {
        if (IsNewGoal)
        {
            Goal = new Goal
            {
                ClientId = ClientVm.Client.Id
                         ,
                DateCompleted = DateTime.Today
                         ,
                DateStarted = StartDatePicker.Date
                         ,
                TargetDate = TargetDatePicker.Date
                         ,
                Description = DescriptionEditor.Text
                         ,
                Failed = FailedCheckbox.IsChecked
                         ,
                Name = NameEditor.Text
                         ,
                SuccessfullyCompleted = SuccessfullyCompletedCheckbox.IsChecked
            };
        }
        else
        {
            //Only set editable properties (e.g. Do not set the ClientId)
            Goal.DateCompleted = DateTime.Today;
            Goal.DateStarted = StartDatePicker.Date;
            Goal.TargetDate = TargetDatePicker.Date;
            Goal.Description = DescriptionEditor.Text;
            Goal.Failed = FailedCheckbox.IsChecked;
            Goal.Name = NameEditor.Text;
            Goal.SuccessfullyCompleted = SuccessfullyCompletedCheckbox.IsChecked;
        }

        ClientVm.Save(Goal);
        PageNavigation.NavigateBackwards();
    }

}