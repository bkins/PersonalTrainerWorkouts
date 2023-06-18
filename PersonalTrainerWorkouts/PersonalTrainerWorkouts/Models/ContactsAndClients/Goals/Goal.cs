using System;
using System.ComponentModel;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;

[Table($"{nameof(Goal)}s")]
public class Goal : BaseModel
{
    [PrimaryKey, AutoIncrement]
    public int       Id            { get; set; }
    public string    Description   { get; set; }
    public DateTime? DateStarted   { get; set; }
    public DateTime? DateCompleted { get; set; }
    public DateTime  TargetDate    { get; set; }

    [Ignore]
    public bool SuccessfullyCompleted { get; set; }
    
    [Ignore]
    public DateTime TodaysDate { get; set; }
    /// <summary>
    /// Used to end the goal without completing it successfully.
    /// "Failed" can be synonymous with "Abandon"
    /// This is the only non-calculated status
    /// </summary>
    public bool              Failed           { get; set; }
    public TargetComparisons TargetComparison { get; set; }
    
    [Ignore] 
    public bool MissedTarget { get; set; }

    [Ignore] 
    public bool InProcess { get; set; }

    [Ignore] 
    public bool NotStarted { get; set; }

    [Ignore]
    public bool HasMeasurables { get; set; }
    
    [ForeignKey(typeof(Client))]
    public int ClientId { get; set; }

    [ForeignKey(typeof(Measurable))]
    public int BaselineMeasurableId { get; set; }
    
    [ForeignKey(typeof(Measurable))]
    public int MeasurableToMeetId { get; set; }

    public Goal()
    {
        TodaysDate = DateTime.Now;
        TargetDate = DateTime.Today.AddMonths(1);
        CalculateStatus();
    }

    public void CalculateStatus()
    {
        NotStarted = DateStarted is null;
        
        MissedTarget = TargetDate.Date < TodaysDate.Date
                    && DateCompleted is null
                    || DateCompleted.GetValueOrDefault().Date > TargetDate.Date;
        
        InProcess = ! MissedTarget 
                 && DateCompleted is null
                 && DateStarted is not null;
        
        SuccessfullyCompleted = ! Failed
                             && ! MissedTarget
                             && ! InProcess
                             && ! NotStarted
                             && ! InProcess;
    }
    
    public Status GetStatus()
    {
        CalculateStatus();
        
        //If Failed was set than override all other statuses to false.
        if (Failed)
        {
            SuccessfullyCompleted = false;
            NotStarted            = false;
            MissedTarget          = false;
            InProcess             = false;
            
            return Status.Failed;
        }

        if (SuccessfullyCompleted) return Status.CompletedSuccessfully;
        if (NotStarted)            return Status.NotStarted;
        if (MissedTarget)          return Status.MissTarget;
        if (InProcess)             return Status.InProgress;

        return Status.Unknown;
    }

    public enum Status
    {
        [Description("Completed")]
        CompletedSuccessfully
        
      , [Description("Failed")]
        Failed
        
      , [Description("Not Started")]
        NotStarted
        
      , [Description("Missed Target")]
        MissTarget
        
      , [Description("In Progress")]
        InProgress
        
      , [Description("Could not determine the status")]
        Unknown
    }

    public override string ToString()
    {
        var status = GetStatus();

        // var formattedStatus = status is Status.Unknown ?
        //                               " - Could not determine the status" :
        //                               $" ({status})"; 
        return $"{Name} - {status}";
    }
}

public enum TargetComparisons
{
    MustBeEqual
  , CanBeLessThan
  , CanBeGreaterThan
}