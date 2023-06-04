using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avails.D_Flat.Extensions;
using Microsoft.Extensions.Primitives;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Clients;

public class GoalViewModel : ViewModelBase
{
    private string _goalStatusImageFileName;
    public Goal                 Goal                  { get; set; }
    public MeasurablesViewModel MeasurablesViewModel  { get; set; }
    public string               GoalStatusDescription { get; set; }

    public string GoalStatusImageFileName
    {
        get => _goalStatusImageFileName;
        set
        {
            _goalStatusImageFileName = value;
            SetValue(ref _goalStatusImageFileName, value, nameof(GoalStatusImageFileName));
        } 
    }

    //Client data
    public string ClientName           { get; set; }
    public string ClientNameForDisplay { get; set; }
    
    public GoalViewModel()
    {
        
    }

    public GoalViewModel(Goal       goal
                       , DataAccess dataAccess)
    {
        _dataAccess = dataAccess;
        Initialize(goal);
    }
    
    public GoalViewModel(Goal goal)
    {
        Initialize(goal);
    }

    private void Initialize(Goal goal)
    {
        if (goal is null) return;

        Goal                 = DataAccessLayer.GetGoal(goal.Id);
        MeasurablesViewModel = new MeasurablesViewModel(goal.Id);
        ClientName           = DataAccessLayer.GetClients().FirstOrDefault(client => client.Id == goal.ClientId)?.Name;
        ClientNameForDisplay = $"{ClientName}'s Goal";

        CalculateStatus();
    }

    private string GetGoalStatusFileName()
    {
        return Goal.GetStatus() switch
            {
                Goal.Status.CompletedSuccessfully => "Success_48.png"
              , Goal.Status.NotStarted            => "NotStarted_48.png"
              , Goal.Status.InProgress            => "InProgress_48.png"
              , Goal.Status.MissTarget            => "MissedTarget_48.png"
              , Goal.Status.Failed                => "Failed_48.png"
              , Goal.Status.Unknown               => "Unknown_48.png"
              , _ => throw new ArgumentOutOfRangeException()
            };
    }

    public void CalculateStatus()
    {
        if (Goal is null ) return;

        SetDateCompletedBasedOnMeasurables();

        GoalStatusDescription   = Goal.GetStatus().GetDescription();
        GoalStatusImageFileName = GetGoalStatusFileName();    
    }

    private void SetDateCompletedBasedOnMeasurables()
    {
        MeasurablesViewModel.Refresh();

        var targetMeasurable = MeasurablesViewModel.Measurables
                                                   .FirstOrDefault(measurables => measurables.GoalSuccession == Succession.Target);

        foreach (var measurable in MeasurablesViewModel.Measurables.Where(measurable=>measurable.GoalSuccession != Succession.Baseline 
                                                                                   && measurable.GoalSuccession != Succession.Target))
        {
            var targetComparison = CompareMeasurableToTarget(measurable, targetMeasurable);

            if (! targetComparison) continue;

            Goal.DateCompleted = measurable.DateTaken;
        }
    }

    private bool CompareMeasurableToTarget(Measurable measurable
                                         , Measurable targetMeasurable)
    {
        const double tolerance   = 0.000000001;

        var targetValue      = targetMeasurable?.Value ?? 0;
        var targetComparison = Goal.TargetComparison switch
                               {
                                   TargetComparisons.MustBeEqual => Math.Abs(measurable.Value - targetValue) < tolerance
                                 , TargetComparisons.CanBeGreaterThan => measurable.Value >= targetValue
                                 , TargetComparisons.CanBeLessThan => measurable.Value <= targetValue
                                 , _ => false
                               };

        return targetComparison;
    }

    public IEnumerable<Measurable> Measurables { get; set; }

    public override string ToString()
    {
        var vmData = new StringBuilder();
        
        vmData.Append($"{nameof(GoalStatusDescription)}: ");
        vmData.AppendLine(GoalStatusDescription);

        vmData.Append($"{nameof(GoalStatusImageFileName)}: ");
        vmData.AppendLine(GoalStatusImageFileName);

        return vmData.ToString();
    }
}