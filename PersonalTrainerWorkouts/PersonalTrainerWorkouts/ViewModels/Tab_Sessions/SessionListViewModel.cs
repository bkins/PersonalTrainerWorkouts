using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.HelperClasses;
using Syncfusion.SfSchedule.XForms;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Sessions
{
    public class SessionListViewModel : ViewModelBase
    {
        public  ObservableCollection<Session>             ObservableListOfSessions { get; set; }

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
        public SessionListViewModel(IEnumerable<Session> aListOfSessions
                                  , DataAccess           dbAccessLayer)
        {
            DataAccessLayer = dbAccessLayer;
            LoadData(aListOfSessions);
        }

        public void LoadData()
        {
            var allSessions = DataAccessLayer.GetSessions();
            LoadData(allSessions);
        }
        public void LoadData(IEnumerable<Session> sessions)
        {
            var allSessions = sessions.ToList();

            ObservableListOfSessions = new ObservableCollection<Session>(allSessions);

            var appointmentsList = new List<SessionAppointment>();

            try
            {
                appointmentsList = allSessions.Select(GetScheduledAppointment)
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

        private static SessionAppointment GetScheduledAppointment(Session session)
        {
            var backColor   = Color.FromHex(session.Client.BackgroundColorHex);
            var textColor   = Color.FromHex(session.Client.TextColorHex);
            var subject     = GetSubject(session);

            var appointment = new SessionAppointment();
            appointment.SessionId              = session.Id;
            appointment.SessionStartTime       = session.StartDate;
            appointment.SessionEndTime         = session.EndDate;
            appointment.SessionSubject         = subject;
            appointment.SessionNotes           = session.Client.MainNumber;
            appointment.SessionBackgroundColor = backColor;
            appointment.SessionTextColor       = textColor;
            appointment.StartTime              = session.StartDate;
            appointment.EndTime                = session.EndDate;
            appointment.Subject                = subject;
            appointment.Color                  = backColor;
            appointment.TextColor              = textColor;

            return appointment;
        }

        private static string GetSubject(Session session)
        {
            var subject = new StringBuilder();
            subject.AppendLine(session.Client.DisplayName);

            if (session.Workouts.Count == 1)
            {
                subject.Append(session.Workouts.First().Name);
            }
            else
            {
                foreach (var workout in session.Workouts)
                {
                    subject.Append($"{workout.Name}; ");
                }

                var subjectString = subject.ToString()
                                           .Remove(subject.ToString()
                                                          .Length - 1
                                                 , 1);

                return subjectString;
            }

            return subject.ToString();
        }
        public (string item, bool success) Delete(int index)
        {
            if (index > ObservableListOfSessions.Count - 1)
            {
                return (string.Empty, false);
            }

            //Get the item to be deleted
            var itemToDelete = ObservableListOfSessions[index];
            var name = itemToDelete.Name;

            //Remove the item from the source list
            ObservableListOfSessions.RemoveAt(index);

            //Delete the item from the database
            var numberAffected = App.Database.DeleteSession(ref itemToDelete);

            LoadData(DataAccessLayer.GetSessions());

            //ObservableListOfSessions = new ObservableCollection<Session>(DataAccessLayer.GetSessions());

            return numberAffected switch
                   {
                         0 => ("<Session was not deleted. See Logs>", false)
                     , > 1 => ("<More than one Session was deleted!", true)
                     , _   => (name, true)
                   };
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

        public ObservableCollection<Session> GetSessionsForTheWeek()
        {
            var today = DateTime.Today;
            var listOfSessions = ObservableListOfSessions.Where(session => session.StartDate >= today
                                                                        && session.StartDate <= today.AddDays(7));

            return new ObservableCollection<Session>(listOfSessions);
        }
    }
}
