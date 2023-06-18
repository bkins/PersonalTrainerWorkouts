using SQLite;

namespace PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;

[Table($"{nameof(GoalSuccession)}s")]
public class GoalSuccession
{
    [PrimaryKey, AutoIncrement]
    public int        Id         { get; set; }
    public Succession Succession { get; set; }
}

public enum Succession
{
    Baseline
  , Target
  , Interim
}