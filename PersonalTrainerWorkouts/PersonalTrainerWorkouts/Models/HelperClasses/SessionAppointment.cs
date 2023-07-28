using System;
using Syncfusion.SfSchedule.XForms;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Models.HelperClasses;

public class SessionAppointment : ScheduleAppointment
{
    public int      SessionId        { get; set; }
    /// <summary>
    /// Background Color
    /// </summary>
    public Color    SessionBackgroundColor     { get; set; }
    public Color    SessionTextColor { get; set; }
    public DateTime SessionStartTime { get; set; }
    public DateTime SessionEndTime   { get; set; }
    public string   SessionSubject   { get; set; }
    public string   SessionNotes     { get; set; }
    public Session  Session   { get; set; }

    public SessionAppointment()
    { }

    public SessionAppointment(Session session)
    {
        SessionId              = session.Id;
        SessionBackgroundColor = session.Client.BackgroundColor;
        SessionTextColor       = session.Client.TextColor;
        SessionStartTime       = session.StartDate;
        SessionEndTime         = session.EndDate;
        SessionSubject         = session.Client.DisplayName;
        SessionNotes           = null + (session.Client.MainNumber + Environment.NewLine);

        Session = session;
    }

    public override string ToString()
    {
        return $"{SessionSubject}: {SessionStartTime.TimeOfDay}-{SessionEndTime.TimeOfDay}";
    }
}
