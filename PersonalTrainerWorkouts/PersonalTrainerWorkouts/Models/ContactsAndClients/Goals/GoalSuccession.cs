using PersonalTrainerWorkouts.ViewModels.HelperClasses;
using SQLite;

namespace PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;

[Table($"{nameof(GoalSuccession)}s")]
public class GoalSuccession
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public Enums.Succession Succession { get; set; }
}
