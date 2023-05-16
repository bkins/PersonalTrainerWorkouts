using SQLite;

using SQLiteNetExtensions.Attributes;

using System;
using PersonalTrainerWorkouts.Models.ContactsAndClients;

namespace PersonalTrainerWorkouts.Models;

[Table($"{nameof(Goal)}s")]
public class Goal : BaseModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Description { get; set; }
    public DateTime? DateStarted { get; set; }
    public DateTime? DateCompleted { get; set; }
    public DateTime TargetDate { get; set; }
    public bool SuccessfullyCompleted { get; set; }

    /// <summary>
    /// Used to end the goal without completing it successfully.
    /// "Failed" can be synonymous with "Abandon"
    /// </summary>
    public bool Failed { get; set; }
    //{
    //    get => ! SuccessfullyCompleted && DateCompleted is null;
    //}

    [Ignore]
    public bool MissedTarget
    {
        get => (TargetDate > DateTime.Now && DateCompleted is null) || DateCompleted > TargetDate;
    }

    [Ignore]
    public bool InProcess
    {
        get => DateCompleted is null
            && DateStarted is not null
            && !Failed
            && !SuccessfullyCompleted;
    }

    [Ignore]
    public bool NotStarted
    {
        get => DateStarted is null;
    }

    [ForeignKey(typeof(Client))]
    public int ClientId { get; set; }

    public string GetStatus()
    {
        if (SuccessfullyCompleted)
        {
            return "Completed Successfully";
        }

        if (Failed)
        {
            return "Failed";
        }

        if (NotStarted)
        {
            return "Not Started";
        }

        if (MissedTarget)
        {
            return "Missed Target";
        }

        if (InProcess)
        {
            return "In Process";
        }

        return null;
    }
    public override string ToString()
    {
        var status = GetStatus();

        var formattedStatus = status is null ?
                                      string.Empty :
                                      $" ({status})"; 
        return $"{Name}{status}";
    }
}