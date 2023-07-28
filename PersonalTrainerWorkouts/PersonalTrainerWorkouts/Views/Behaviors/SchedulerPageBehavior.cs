using Avails.Xamarin;
using PersonalTrainerWorkouts.Models.HelperClasses;
using PersonalTrainerWorkouts.Views.Tab_Sessions;
using Syncfusion.SfSchedule.XForms;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Views.Behaviors;

public class SchedulerPageBehavior : Behavior<ContentPage>
{
    private const string NameOfSfScheduleControl = "NewSessionSchedule";

    private SfSchedule  _schedule;
    private ContentPage _contentPage;

    protected override void OnBindingContextChanged()
    {
        var test = string.Empty;

        base.OnBindingContextChanged();
    }

    protected override void OnAttachedTo(ContentPage bindable)
    {
        base.OnAttachedTo(bindable);

        _contentPage = bindable;
        _schedule    = bindable.FindByName<SfSchedule>(NameOfSfScheduleControl);

        WireEvents();
    }

    private void WireEvents()
    {
        _schedule.CellTapped += Schedule_CellTapped;
    }

    private async void Schedule_CellTapped(object sender, CellTappedEventArgs e)
    {
        if (e.Appointment == null) return;
        var appointment = (SessionAppointment)e.Appointment;

        var title = appointment.SessionSubject;
        var notes = appointment.SessionNotes;

        var accepted = await _contentPage.DisplayAlert(title
                                               , $"{notes}"
                                               , "Text" /*accept*/
                                               , "Edit/View" /*cancel*/)
                                         .ConfigureAwait(false);
        if (accepted)
        {
            await Sms.ComposeAsync(new SmsMessage(string.Empty
                                                , appointment.SessionNotes));
        }
        else
        {
            var instance = new SessionEditPage();

            PageNavigation.NavigateTo(instance);
            // PageNavigation.NavigateTo(nameof(SessionEditPage)
            //                         , nameof(SessionEditPage.SessionId)
            //                         , appointment.SessionId.ToString());
        }
    }

    protected override void OnDetachingFrom(ContentPage bindable)
    {
        base.OnDetachingFrom(bindable);
        UnWireEvents();
    }

    private void UnWireEvents()
    {
        _schedule.CellTapped -= Schedule_CellTapped;
    }
}
