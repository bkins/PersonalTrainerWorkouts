using System;
using System.Collections.Generic;
using System.Linq;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Clients;

public class MeasurablesViewModel : ViewModelBase
{
    private readonly int _goalId = 0;

    public IEnumerable<Measurable> Measurables   { get; set; }
    public Measurable              NewMeasurable { get; set; }
    public Goal                    Goal          { get; set; }
    
    public MeasurablesViewModel(int goalId)
    {
        NewMeasurable = new Measurable();
        _goalId       = goalId;
        Goal          = _goalId == 0 ? null : DataAccessLayer.GetGoal(_goalId);
        
        Refresh();
    }

    public MeasurablesViewModel(int       goalId
                              , DataAccess dataAccess)
    {
        NewMeasurable = new Measurable();
        _dataAccess   = dataAccess;
        _goalId       = goalId;
        Goal          = _goalId == 0 ? null : DataAccessLayer.GetGoal(_goalId);
        
        Refresh();
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
                               , int        unitOfMeasurementId
                               , int        goalId = 0)
    {
        NewMeasurable = new Measurable
                        {
                            Variable            = variable
                          , Value               = value
                          , DateTaken           = dateTaken
                          , Type                = type
                          , GoalSuccession      = goalSuccession
                          , ClientId            = clientId
                          , UnitOfMeasurementId = unitOfMeasurementId
                          , GoalId              = goalId == 0 ? _goalId : goalId
                        };
        DataAccessLayer.AddMeasurable(NewMeasurable);
    }
    
    public void AddBaselineMeasurable(string   variable
                                    , double   value
                                    , DateTime dateTaken
                                    , string   type
                                    , int      clientId
                                    , int      unitOfMeasurementId)
    {
        AddNewMeasurable(variable, value, dateTaken, type, Succession.Baseline, clientId, unitOfMeasurementId);
    }
    
    public void AddTargetMeasurable(Measurable baselineMeasurable, double targetValue)
    {
        AddNewMeasurable(baselineMeasurable.Variable
                       , targetValue
                       , baselineMeasurable.DateTaken
                       , baselineMeasurable.Type
                       , Succession.Target
                       , baselineMeasurable.ClientId
                       , baselineMeasurable.UnitOfMeasurementId);
    }

    public void AddInterimMeasurable(Measurable baselineMeasurable
                                   , double     newValue
                                   , DateTime   dateTake)
    {
        AddNewMeasurable(baselineMeasurable.Variable
                       , newValue
                       , dateTake
                       , baselineMeasurable.Type
                       , Succession.Interim
                       , baselineMeasurable.ClientId
                       , baselineMeasurable.UnitOfMeasurementId);
    }

    #endregion

    public bool Delete(Measurable measurable)
    {
        var numberDeleted = DataAccessLayer.DeleteMeasurable(measurable);

        return numberDeleted != 1;
    }
}