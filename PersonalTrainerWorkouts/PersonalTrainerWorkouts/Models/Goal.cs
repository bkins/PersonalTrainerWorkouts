using SQLite;

using SQLiteNetExtensions.Attributes;

using System;

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
}