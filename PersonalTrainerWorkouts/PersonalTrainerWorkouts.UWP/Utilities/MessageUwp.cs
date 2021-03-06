using System;
using Microsoft.Toolkit.Uwp.Notifications;
using NLog;
using PersonalTrainerWorkouts.Utilities.Interfaces;

namespace PersonalTrainerWorkouts.UWP.Utilities
{
    class MessageUwp : IMessage
    {
        public void LongAlert(string  message)
        {
            new ToastContentBuilder()
                .SetToastDuration(ToastDuration.Long)
                .AddText(message)
                .Show();
        }

        public void ShortAlert(string message)
        {
            new ToastContentBuilder()
                .SetToastDuration(ToastDuration.Short)
                .AddText(message)
                .Show(); 
        }

        public void Log(LogLevel  level
                      , string    message
                      , Exception ex)
        {
            LongAlert(ex.ToString());
        }
    }
}
