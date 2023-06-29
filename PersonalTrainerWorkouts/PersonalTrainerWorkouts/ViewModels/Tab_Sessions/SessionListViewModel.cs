using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts;
using Syncfusion.DataSource.Extensions;
using Syncfusion.SfSchedule.XForms;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Sessions
{
    public class SessionListViewModel : ViewModelBase
    {
        public ObservableCollection<Session>     ObservableListOfSessions { get; set; }
        public List<WorkoutsToExerciseViewModel> ListOfSessionsWithTotals { get; set; }

        private ObservableCollection<ScheduleAppointment> _appointments;

        public ObservableCollection<ScheduleAppointment> Appointments
        {
            get { return _appointments ??= new ObservableCollection<ScheduleAppointment>(); }
            set => _appointments = value;
        }

        public SessionListViewModel()
        {
            var allSessions = DataAccessLayer.GetSessions();
            LoadData(allSessions);
        }

        /// <summary>
        /// For testing
        /// </summary>
        /// <param name="aListOfSessions"></param>
        /// <param name="dbAccessLayer"></param>
        public SessionListViewModel(List<Session> aListOfSessions, DataAccess dbAccessLayer)
        {
            DataAccessLayer = dbAccessLayer;
            LoadData(aListOfSessions);
        }

        public void LoadData(IEnumerable<Session> sessions)
        {
            var sessionsList = sessions.ToList();

            ObservableListOfSessions = new ObservableCollection<Session>(sessionsList);

            var appointmentsList = new List<ScheduleAppointment>();

            try
            {
                appointmentsList = sessionsList.Select(GetScheduledAppointment)
                                               .ToList();
            }
            catch (Exception e)
            {
                //Think I have fixed the cases when this error was occurring,
                //but will leave the try/catch just in case
                Logger.WriteLine("Error while creating appointmentList.", Category.Error, e);
            }

            Appointments = new ObservableCollection<ScheduleAppointment>(appointmentsList);
        }

        private static ScheduleAppointment GetScheduledAppointment(Session session)
        {
            var appointment = new ScheduleAppointment();
            appointment.StartTime = session.StartDate;
            appointment.EndTime   = session.EndDate;
            appointment.Subject   = session.Client.DisplayName;
            appointment.Notes     = session.Client.MainNumber;
            appointment.Color     = Color.FromHex(session.Client.BackgroundColorHex);
            appointment.TextColor = Color.FromHex(session.Client.TextColorHex);

            return appointment;
        }

        public (string item, bool success) Delete(int index)
        {
            if (index > ObservableListOfSessions.Count - 1)
            {
                return (string.Empty, false);
            }

            //Get the workout to be deleted
            var itemToDelete = ObservableListOfSessions[index];
            var name = itemToDelete.Name;

            //Remove the workout from the source list
            ObservableListOfSessions.RemoveAt(index);

            //Delete the Workout from the database
            var numberAffected = App.Database.DeleteSession(ref itemToDelete);

            ObservableListOfSessions = new ObservableCollection<Session>(DataAccessLayer.GetSessions());

            if (numberAffected == 0) { return ("<Session was not deleted. See Logs>", false); }
            if (numberAffected > 1) { return ("<More than one Session was deleted!", true); }

            return (name, true);
        }

        public ObservableCollection<Session> SearchByClientName(string filterText)
        {
            return new ObservableCollection<Session>
                   (
                        ObservableListOfSessions.Where(field => field.Client
                                                                   .DisplayName
                                                                   .ToUpper()
                                                                   .Contains(filterText.ToUpper()))
                   );
        }

        public ObservableCollection<Session> SearchSessions(string filterText)
        {

            return SearchByClientName(filterText);
        }
    }
}
