using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.HelperClasses;
using PersonalTrainerWorkouts.Views.Tab_Sessions;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Sessions;

public class SchedulerViewModel : ViewModelBase, INotifyPropertyChanged
    {
        /// <summary>
        /// collections for meetings.
        /// </summary>
        private ObservableCollection<SessionAppointment> _appointments;

        /// <summary>
        /// color collection.
        /// </summary>
        private List<Color> _colorCollection;

        /// <summary>
        /// current day meeting.
        /// </summary>
        private List<string> _currentDayAppointments;

        public SchedulerViewModel()
        {
            AppointmentDataTemplate = new DataTemplate(() => new AppointmentDataTemplate());

            var allSessions      = DataAccessLayer.GetSessions();
            var sessionsList     = allSessions.ToList();
            var appointmentsList = new List<SessionAppointment>();

            try
            {
                appointmentsList = sessionsList.Select(GetScheduledAppointment)
                                               .ToList();
            }
            catch (Exception e)
            {
                //Think I have fixed the cases when this error was occurring,
                //but will leave the try/catch just in case
                Console.WriteLine("Error while creating appointmentList.");
            }

            Appointments = new ObservableCollection<SessionAppointment>(appointmentsList);

            //Appointments = new ObservableCollection<SessionAppointment>(appointmentsList);
            //AddAppointmentDetails();
            //AddAppointments();
        }
        private static SessionAppointment GetScheduledAppointment(Session session)
        {
            var appointment = new SessionAppointment(session);
            // appointment.StartTime = session.StartDate;
            // appointment.EndTime   = session.EndDate;
            // appointment.Subject   = session.Client.DisplayName;
            // appointment.Notes     = session.Client.MainNumber;
            // appointment.Color     = Color.FromHex(session.Client.BackgroundColorHex);
            // appointment.TextColor = Color.FromHex(session.Client.TextColorHex);

            return appointment;
        }

        /// <summary>
        /// Gets or sets meetings.
        /// </summary>
        public ObservableCollection<SessionAppointment> Appointments
        {
            get => _appointments;
            set
            {
                _appointments = value;
                RaiseOnPropertyChanged(nameof(Appointments));
            }
        }

        /// <summary>
        /// Gets or sets the AppointmentDataTemplate.
        /// </summary>
        public DataTemplate AppointmentDataTemplate { get; set; }

        /// <summary>
        /// adding appointment details.
        /// </summary>
        private void AddAppointmentDetails()
        {
            _currentDayAppointments = new List<string>();
            _currentDayAppointments.Add("General Meeting");
            _currentDayAppointments.Add("Plan Execution");
            _currentDayAppointments.Add("Project Plan");
            _currentDayAppointments.Add("Consulting");
            _currentDayAppointments.Add("Support");
            _currentDayAppointments.Add("Development Meeting");
            _currentDayAppointments.Add("Scrum");
            _currentDayAppointments.Add("Project Completion");
            _currentDayAppointments.Add("Release updates");
            _currentDayAppointments.Add("Performance Check");

            _colorCollection = new List<Color>();
            _colorCollection.Add(Color.FromHex("#FFA2C139"));
            _colorCollection.Add(Color.FromHex("#FFD80073"));
            _colorCollection.Add(Color.FromHex("#FF1BA1E2"));
            _colorCollection.Add(Color.FromHex("#FFE671B8"));
            _colorCollection.Add(Color.FromHex("#FFF09609"));
            _colorCollection.Add(Color.FromHex("#FF339933"));
            _colorCollection.Add(Color.FromHex("#FF00ABA9"));
            _colorCollection.Add(Color.FromHex("#FFE671B8"));
            _colorCollection.Add(Color.FromHex("#FF1BA1E2"));
            _colorCollection.Add(Color.FromHex("#FFD80073"));
            _colorCollection.Add(Color.FromHex("#FFA2C139"));
            _colorCollection.Add(Color.FromHex("#FFA2C139"));
            _colorCollection.Add(Color.FromHex("#FFD80073"));
            _colorCollection.Add(Color.FromHex("#FF339933"));
            _colorCollection.Add(Color.FromHex("#FFE671B8"));
            _colorCollection.Add(Color.FromHex("#FF00ABA9"));
        }

        /// <summary>
        /// Adds the appointments.
        /// </summary>
        private void AddAppointments()
        {
            var today = DateTime.Now.Date;
            var random = new Random();
            for (var month = -1; month < 2; month++)
            {
                for (var day = -5; day < 5; day++)
                {
                    for (var count = 0; count < 2; count++)
                    {
                        var startDate = today.AddMonths(month)
                                             .AddDays(random.Next(1, 28))
                                             .AddHours(random.Next(9, 18));
                        var session = new Session();
                        session.StartDate                 = startDate;
                        session.EndDate                   = startDate.AddHours(1);
                        //session.Note                      = $"{_currentDayAppointments.ToArray()[random.Next(7)]}{Environment.NewLine}";
                        session.Client.BackgroundColorHex = _colorCollection.ToArray()[random.Next(14)].ToHex();

                        var meeting = new SessionAppointment(session);
                        // meeting.StartTime        = today.AddMonths(month).AddDays(random.Next(1, 28)).AddHours(random.Next(9, 18));
                        // meeting.EndTime          = meeting.StartTime;
                        // meeting.Notes            = $"{_currentDayAppointments[random.Next(7)]}{Environment.NewLine}";
                        // meeting.Color            = _colorCollection[random.Next(14)];
                        Appointments.Add(meeting);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public new event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoke method when property changed.
        /// </summary>
        /// <param name="propertyName">property name</param>
        private void RaiseOnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
