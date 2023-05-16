using PersonalTrainerWorkouts.Models;

using System;

namespace PersonalTrainerWorkouts.ViewModels;

public class GoalViewModel : BaseViewModel
{
    public Goal Goal { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? DateStarted { get; set; }
    public DateTime? DateCompleted { get; set; }
    public DateTime TargetDate { get; set; }
    public bool SuccessfullyCompleted { get; set; }
    public bool Failed { get; set; }

    //Calculated fields (not stored in the DB)
    public bool MissedTarget { get; }
    public bool InProcess { get; }
    public bool NotStarted { get; }

    public GoalViewModel()
    {
        
    }

    public GoalViewModel(Goal goal)
    {
        if (goal is null)
        {
            return;
        }

        Goal = DataAccessLayer.GetGoal(goal.Id);

        Description = goal.Description;
        DateStarted = goal.DateStarted;
        DateCompleted = goal.DateCompleted;
        TargetDate = goal.TargetDate;
        SuccessfullyCompleted = goal.SuccessfullyCompleted;

        Failed = goal.Failed;
        MissedTarget = goal.MissedTarget;
        InProcess = goal.MissedTarget;
        NotStarted = goal.NotStarted;
    }
}