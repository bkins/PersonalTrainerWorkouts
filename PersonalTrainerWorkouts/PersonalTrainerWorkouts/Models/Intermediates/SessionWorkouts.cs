using SQLiteNetExtensions.Attributes;

namespace PersonalTrainerWorkouts.Models.Intermediates;

public class SessionWorkouts
{
    [ForeignKey(typeof(Session))]
    public int SessionId { get; set; }

    [ForeignKey(typeof(Workout))]
    public int WorkoutId { get; set; }
}
