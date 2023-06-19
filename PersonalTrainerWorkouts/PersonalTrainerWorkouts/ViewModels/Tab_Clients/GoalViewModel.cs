using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avails.D_Flat.Extensions;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;
using PersonalTrainerWorkouts.ViewModels.HelperClasses;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Clients;

public class GoalViewModel : ViewModelBase
{
    private string _goalStatusImageFileName;

    public Goal Goal { get; set; }

    // public Measurable                 Measurable            { get; set; }
    //public MeasurablesViewModel       MeasurablesViewModel  { get; set; }
    public List<MeasurablesViewModel>                 MeasurablesViewModels               { get; set; }
    public IEnumerable<MeasurablesViewModel>          MeasurablesVmList                   { get; set; }
    public IEnumerable<IGrouping<string, MeasurablesViewModel>> MeasurablesViewModelGroupByVariable => MeasurablesVmList
                                                                                                       .Where(measurable => measurable.GoalSuccession
                                                                                                                         != Succession.Baseline)
                                                                                                       .OrderBy(measurable => measurable.Variable)
                                                                                                       .ThenByDescending(
                                                                                                           measurable => measurable.GoalSuccession)
                                                                                                       .ThenBy(measurable => measurable.DateTaken)
                                                                                                       .GroupBy(measurable => measurable.Variable);
    public string                                     GoalStatusDescription               { get; set; }

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
        Goal                 = new Goal();
        //Measurable           = new Measurable();
        //MeasurablesViewModel = new MeasurablesViewModel(0);
    }

    public GoalViewModel(int clientId)
    {
        SetListOfMeasurableViewModelsForClient(clientId);
    }

    public GoalViewModel(DataAccess dataAccess)
    {
        _dataAccess          = dataAccess;
        Goal                 = new Goal();
        //Measurable           = new Measurable();
        //MeasurablesViewModel = new MeasurablesViewModel(0);
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

    private void Initialize(Goal goal, bool refreshGoal = false)
    {
        if (goal is null) return;

        Goal = refreshGoal
                    ? DataAccessLayer.GetGoal(goal.Id)
                    : goal;

        SetListOfMeasurableViewModelsForGoal();

        ClientName = DataAccessLayer.GetClients()
                                    .FirstOrDefault(client => client.Id == goal.ClientId)
                                    ?.Name;

        ClientNameForDisplay = $"{ClientName}'s Goal";

        CalculateStatus();
    }
    private void SetListOfMeasurableViewModelsForGoal()
    {
        MeasurablesViewModels = new List<MeasurablesViewModel>();
        // foreach (var goal in Goals)
        // {
        //     foreach (var measurable in goal.Measurables)
        //     {
        //         Measurables.Add(new MeasurablesViewModel(goal.Goal.Id, measurable.Id));
        //     }
        //
        // }
        var allMeasurables = new StringBuilder();
        var measurables = DataAccessLayer.GetMeasurables()
                                         .Where(measurable => measurable.GoalId == Goal.Id)
                                         .ToList();
        foreach (var measurable in measurables)
        {
            var thisMeasurable = new MeasurablesViewModel(measurable.GoalId, measurable.ClientId);
            thisMeasurable.NewMeasurable ??= new Measurable();

            MeasurablesViewModels.Add(thisMeasurable);
            //allMeasurables.AppendLine(thisMeasurable.NewMeasurable.ToString());
        }

        //Logger.WriteLine(allMeasurables.ToString(), Category.Information);
    }

    private void SetListOfMeasurableViewModelsForClient(int clientId)
    {
        MeasurablesViewModels = new List<MeasurablesViewModel>();

        var allMeasurables = new StringBuilder();
        var measurables = DataAccessLayer.GetMeasurables()
                                         .Where(measurable => measurable.ClientId == clientId)
                                         .ToList();
        foreach (var measurable in measurables)
        {
            var thisMeasurable = new MeasurablesViewModel(measurable.ClientId);
            thisMeasurable.NewMeasurable ??= new Measurable();

            MeasurablesViewModels.Add(thisMeasurable);
        }

        MeasurablesVmList = MeasurablesViewModels;
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
        MeasurablesVmList = UpdateMeasurables();

        if ( ! MeasurablesVmList.Any()) return;

        var targetMeasurable = GetTargetMeasurable();
        var interimMeasurables = GetInterimMeasurables().ToList();

        if (targetMeasurable is null || ! interimMeasurables.Any()) return;

        var interimThatMetGoal = (from measurable in interimMeasurables
                                  let isComplete = CompareMeasurableToTarget(measurable
                                                                           , targetMeasurable)
                                  where isComplete
                                  select measurable).FirstOrDefault();
        if (interimThatMetGoal is not null)
        {
            Goal.DateCompleted = interimThatMetGoal.DateTaken;
        }
        foreach (var measurable in
                 from measurable in interimMeasurables
                 let isComplete = CompareMeasurableToTarget(measurable, targetMeasurable)
                 where isComplete
                 select measurable)
        {
            Goal.DateCompleted = measurable.DateTaken;
        }
    }

    private IEnumerable<MeasurablesViewModel> UpdateMeasurables()
    {
        var measurables = DataAccessLayer.GetMeasurablesByGoal(Goal.Id)
                              .ToList();
        var measurableViewModels = measurables.Select(measurable => new MeasurablesViewModel(measurable.Variable
                                                                                           , measurable.Value
                                                                                           , measurable.Type
                                                                                           , measurable.DateTaken
                                                                                           , measurable.GoalSuccession
                                                                                           , measurable.UnitOfMeasurement))
                                              .ToList();
        return measurableViewModels;
    }

    private IEnumerable<MeasurablesViewModel> GetInterimMeasurables()
    {
        var interimMeasurables = MeasurablesVmList.Where(measurable => measurable.GoalSuccession != Succession.Baseline
                                                                    && measurable.GoalSuccession != Succession.Target);

        return interimMeasurables;
    }

    private MeasurablesViewModel GetTargetMeasurable()
    {
        var targetMeasurable = MeasurablesVmList.FirstOrDefault(measurable => measurable.GoalSuccession == Succession.Target);

        return targetMeasurable;
    }

    public bool CompareMeasurableToTarget(MeasurablesViewModel measurable
                                        , MeasurablesViewModel targetMeasurable)
    {
        return CompareMeasurableToTarget(measurable.Value
                                       , targetMeasurable.Value
                                       , Goal.TargetComparison);
    }

    //TODO: Consider moving this logic out of the ViewModel
    public static bool CompareMeasurableToTarget(double measurableValue
                                               , double targetValue
                                               , TargetComparisons    targetComparison)
    {
        var target = targetValue;
        var comparison = targetComparison switch
                         {
                             TargetComparisons.MustBeEqual => Math.Abs(measurableValue - targetValue) < Constants.Tolerance
                           , TargetComparisons.CanBeGreaterThan => measurableValue >= targetValue
                           , TargetComparisons.CanBeLessThan => measurableValue <= targetValue
                           , _ => false
                         };

        return comparison;
    }

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
