using System;
using System.Collections.Generic;
using System.Linq;
using Avails.ApplicationExceptions;
using Avails.D_Flat.Extensions;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;
using PersonalTrainerWorkouts.ViewModels.HelperClasses;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Clients;

public class MeasurablesViewModel : ViewModelBase
    /* TODO: Consider creating an Interface for each model
             so that the ViewModels can inherit from the
             interfaces instead of having to copy/paste
             the properties of the models.*/
{
    private readonly int _goalId = 0;
    private readonly int _measurableId = 0;

    public int                      Id                { get; set; }
    public string                   Variable          { get; set; }
    public double                   Value             { get; set; }
    public DateTime                 DateTaken         { get; set; }
    public string                   Type              { get; set; }
    public Succession               GoalSuccession    { get; set; }
    public char                     SuccessionChar    => GoalSuccession.ToString()[0];
    public string                   UnitOfMeasurement { get; set; }
    public IEnumerable<Measurable>  Measurables       { get; set; }
    public Measurable               NewMeasurable     { get; set; }
    public Goal                     Goal              { get; set; }
    public Enums.MeasurableStatuses Status            => GetMeasurableStatus();
    public string                   StatusImageFile   => GetMeasurableStatusImageFileName();
    public bool                     IsTarget          => GoalSuccession == Succession.Target;
    public bool                     IsInterim         => GoalSuccession == Succession.Interim;

    public (string DefaultName, int CountOfUniqueVariable) DefaultVariableName => GetDefaultVariableName();

    public MeasurablesViewModel()
    {

    }

    public MeasurablesViewModel(string     variable
                              , double     value
                              , string     type
                              , DateTime   dateTaken
                              , Succession goalSuccession
                              , string     unitOfMeasurement)
    {
        Variable          = variable;
        Value             = value;
        Type              = type;
        DateTaken         = dateTaken;
        GoalSuccession    = goalSuccession;
        UnitOfMeasurement = unitOfMeasurement;
    }


    public MeasurablesViewModel(int clientId)
    {
        NewMeasurable = new Measurable();
        _goalId       = 0;
        _measurableId = 0;
        Goal          = null;
        // GoalViewModel = new GoalViewModel();

        if (clientId == 0) return;

        Measurables   = DataAccessLayer.GetMeasurablesByClient(clientId);
    }

    public MeasurablesViewModel(int goalId, int measurableId)
    {
        _goalId       = goalId;
        _measurableId = measurableId;

        SetMeasurables(goalId);

        Goal = _goalId == 0 ? null : DataAccessLayer.GetGoal(_goalId);
        // GoalViewModel = new GoalViewModel(Goal);

        //The default of the GoalSuccession is Interim.
        GoalSuccession = Succession.Interim;
        Refresh();
    }

    public MeasurablesViewModel(int        goalId
                              , int        measurableId
                              , DataAccess dataAccess)
    {
        _dataAccess = dataAccess;

        NewMeasurable = DataAccessLayer.GetMeasurable(measurableId);
        _goalId       = goalId;
        Goal          = _goalId == 0 ? null : DataAccessLayer.GetGoal(_goalId);
        // GoalViewModel = new GoalViewModel(Goal);

        Refresh();
    }

    private void SetNewMeasurable(int goalId
                                , int measurableId)
    {

        NewMeasurable = DataAccessLayer.GetMeasurable(measurableId);
        if (NewMeasurable.Id == 0)
        {
            NewMeasurable = DataAccessLayer.GetMeasurables()
                                           .FirstOrDefault(measurable => measurable.GoalId == goalId);
        }
    }

    private void SetMeasurables(int goalId)
    {
        var allMeasurables = DataAccessLayer.GetMeasurables().ToList();
        Measurables = allMeasurables.Where(measurable => measurable.GoalId == goalId);
        try
        {

            var defaultVariable = DefaultVariableName.DefaultName;

            if (defaultVariable.HasValue())
            {
                Variable = defaultVariable;
            }
        }
        catch (Exception e)
        {
            Logger.WriteLine("Just log error for now. Fix this", Category.Error);
        }

        //
        // if (_measurableId == 0)
        // {
        //     Measurables = allMeasurables.Where(measurable => measurable.GoalId == goalId);
        // }
        // else
        // {
        //     Measurables = allMeasurables.Where(measurable => measurable.GoalId == goalId
        //                                                   && measurable.Id == _measurableId);
        // }
    }

    private string GetMeasurableStatusImageFileName()
    {
        var status = GetMeasurableStatus();

        return status switch
               {
                   Enums.MeasurableStatuses.StayingTheSame => "stayingTheSame_black_48.png"
                 , Enums.MeasurableStatuses.Improving => "improving_black_48.png"
                 , _ => "notImproving_black_48.png"
               };
    }

    public bool IsImproving()
    {
        var comparables = GetComparableMeasurables();

        return IsImproving(comparables);
    }

    public bool IsImproving((Measurable baseline, Measurable mostRecent) comparables)
    {
        if (_goalId == 0) return false;

        return GoalViewModel.CompareMeasurableToTarget(comparables.baseline.Value
                                                     , comparables.mostRecent.Value
                                                     , Goal.TargetComparison);
    }

    public bool IsNotImproving()
    {
        var comparables = GetComparableMeasurables();

        return IsNotImproving(comparables);
    }

    public bool IsNotImproving((Measurable baseline, Measurable mostRecent) comparables)
    {
        return ! IsImproving(comparables);
    }

    public bool IsStayingTheSame()
    {
        var comparables = GetComparableMeasurables();

        return IsStayingTheSame(comparables);
    }

    public bool IsStayingTheSame((Measurable baseline, Measurable mostRecent) comparables)
    {
        if (_goalId == 0) return false;

        return Math.Abs(comparables.baseline.Value - comparables.mostRecent.Value) < Constants.Tolerance;
    }

    public Enums.MeasurableStatuses GetMeasurableStatus()
    {
        var comparables = GetComparableMeasurables();

        if (_goalId == 0 || IsStayingTheSame(comparables))
        {
            return Enums.MeasurableStatuses.StayingTheSame;
        }

        return IsImproving(comparables)
                ? Enums.MeasurableStatuses.Improving
                : Enums.MeasurableStatuses.NotImproving;
    }

    public (Measurable baseline, Measurable mostRecent) GetComparableMeasurables()
    {
        var baseline = Measurables.FirstOrDefault(measurable => measurable.GoalSuccession == Succession.Baseline);
        var mostRecent = Measurables.OrderByDescending(measurable => measurable.DateTaken)
                                    .Take(1)
                                    .FirstOrDefault();

        return (baseline, mostRecent);
    }
    public void Refresh()
    {
        if (_goalId == 0)
        {
            Measurables = DataAccessLayer.GetMeasurables();
            return;
        }
        
        Measurables = DataAccessLayer.GetMeasurablesByGoal(_goalId);
    }

    #region Adds

    /// <summary>
    /// Use to add a Measurable that is not assigned to a goal
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    /// <param name="dateTaken"></param>
    /// <param name="type"></param>
    /// <param name="goalSuccession"></param>
    /// <param name="clientId"></param>
    /// <param name="unitOfMeasurementId"></param>
    /// <param name="goalId">Set this to a zero to assign this measurable to a goal.
    ///                      Measurables are linked to Goals by the GoalId defined in the constructor</param>
    public void AddNewMeasurable(string     variable
                               , double     value
                               , DateTime   dateTaken
                               , string     type
                               , Succession goalSuccession
                               , int        clientId
                               , string     unitOfMeasurement
                               , int        goalId = 0)
    {
        NewMeasurable = new Measurable
                        {
                            Variable          = variable
                          , Value             = value
                          , DateTaken         = dateTaken
                          , Type              = type
                          , GoalSuccession    = goalSuccession
                          , ClientId          = clientId
                          , UnitOfMeasurement = unitOfMeasurement
                          , GoalId            = goalId == 0 ? _goalId : goalId
                        };
        DataAccessLayer.AddMeasurable(NewMeasurable);
    }

    public void AddBaselineMeasurable(string   variable
                                    , DateTime dateTaken
                                    , string   type
                                    , int      clientId
                                    , string   unitOfMeasurement)
    {
        AddNewMeasurable(variable
                       , 0          //For now, all baseline measurables will have 0, until there is a need for it to have a value.
                       , dateTaken
                       , type
                       , Succession.Baseline
                       , clientId
                       , unitOfMeasurement);
    }

    public void AddTargetMeasurable(Measurable baselineMeasurable, double targetValue)
    {
        AddNewMeasurable(baselineMeasurable.Variable
                       , targetValue
                       , baselineMeasurable.DateTaken
                       , baselineMeasurable.Type
                       , Succession.Target
                       , baselineMeasurable.ClientId
                       , baselineMeasurable.UnitOfMeasurement);
    }

    public (string Reason, string VariableShouldBeUsed) AddInterimMeasurable(Measurable baselineMeasurable
                                                                           , double     newValue
                                                                           , DateTime   dateTake
                                                                           , string     type
                                                                           , string     variable
                                                                           , int        clientId
                                                                           , string     unitOfMeasurement
                                                                           , int        goalId)
    {

        string typeToUse;
        string variableToUse;
        int    clientIdToUse;
        string unitOfMeasurementToUse;
        int    goalIdToUse;

        if (baselineMeasurable is null)
        {
            typeToUse              = type;
            variableToUse          = variable;
            clientIdToUse          = clientId;
            unitOfMeasurementToUse = unitOfMeasurement;
            goalIdToUse            = goalId;
        }
        else
        {
            typeToUse              = baselineMeasurable.Type;
            variableToUse          = baselineMeasurable.Variable;
            clientIdToUse          = baselineMeasurable.ClientId;
            unitOfMeasurementToUse = baselineMeasurable.UnitOfMeasurement;
            goalIdToUse            = baselineMeasurable.GoalId;
        }

        if (DefaultVariableName.CountOfUniqueVariable == 1
         && DefaultVariableName.DefaultName != variableToUse)
        {
            return (
                $"Currently, you cannot add a new type of Measurable to Goals.  I have updated the Variable name with the one you can use. If you want to have multiple variables that need to be met to satisfy the Goal, let's talk about doing that. But for now, that cannot be done."
              , DefaultVariableName.DefaultName);
        }
        AddNewMeasurable(variableToUse
                       , newValue
                       , dateTake
                       , typeToUse
                       , Succession.Interim
                       , clientIdToUse
                       , unitOfMeasurementToUse
                       , goalIdToUse);

        return (string.Empty, string.Empty);
    }

    private (string DefaultName, int CountOfUniqueVariable) GetDefaultVariableName()
    {

        var groupByMeasurablesByVariable = DataAccessLayer.GetMeasurables()
                                                          .Where(m => m.GoalId == Goal.Id)
                                                          .GroupBy(m => m.Variable)
                                                          .ToList();
        var variableShouldBeUsed = groupByMeasurablesByVariable.First().Key;

        return (variableShouldBeUsed, groupByMeasurablesByVariable.Count);
    }

    #endregion

    public bool Save(Measurable measurable)
    {
        if (measurable.Id == 0)
        {
            return DataAccessLayer.AddMeasurable(measurable) > 0;
        }
        DataAccessLayer.UpdateMeasurable(measurable);

        return true;
    }

    public void Save()
    {
        Save(NewMeasurable);
    }

    public bool Delete(Measurable measurable)
    {
        var numberDeleted = DataAccessLayer.DeleteMeasurable(measurable);

        return numberDeleted != 1;
    }

    public void ValidateMeasurable()
    {
        var validUnitOfMeasurements = DataAccessLayer.GetUnitOfMeasures();

        if (validUnitOfMeasurements.All(unit=>unit.Unit != NewMeasurable.UnitOfMeasurement))
        {
            throw new InvalidMeasurable($"'{NewMeasurable.UnitOfMeasurement}' is not a valid {nameof(UnitOfMeasurement)}.");
        }

        if (NewMeasurable.Type != "Max"
            && NewMeasurable.Type != "Measurement")
        {
            throw new InvalidMeasurable($"'{NewMeasurable.Type}' is not a valid {nameof(NewMeasurable.Type)}.");
        }
    }

    public override string ToString()
    {
        return $"{SuccessionChar} - {Variable} - {Value}";
    }
}
