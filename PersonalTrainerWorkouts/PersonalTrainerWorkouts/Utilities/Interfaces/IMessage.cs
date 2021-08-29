using System;
using NLog;
namespace PersonalTrainerWorkouts.Utilities.Interfaces
{
    //https://stackoverflow.com/questions/35279403/toast-equivalent-for-xamarin-forms/44126899#44126899
    public interface IMessage
    {
        void LongAlert(string  message);
        void ShortAlert(string message);

        void Log(LogLevel  level
               , string    message
               , Exception ex);
    }
}
