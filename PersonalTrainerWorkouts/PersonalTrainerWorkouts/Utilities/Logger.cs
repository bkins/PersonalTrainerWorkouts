using System;
using System.IO;
using System.Reflection;
using Xamarin.Forms;
using System.Diagnostics;
using System.Text;
using NLog;
using PersonalTrainerWorkouts.Utilities.Interfaces;
using Switch = System.Diagnostics.Switch;

namespace PersonalTrainerWorkouts.Utilities
{
    public static class Logger
    {
        private static bool _writeToOutput  = true;
        private static bool _writeToConsole = true;
        private static bool _writeToFile    = true;
        private static bool _writeToToast   = true;
        private static bool _writeToLogCat  = true;
        private static bool _verbose        = true;

        public static  StringBuilder Log { get; }

        public static string CompleteLog => Log.ToString();

        static Logger()
        {
            Log = new StringBuilder();
        }

        public static void WriteLine(string message, Category category, Exception ex = null)
        {
            var completeLogMessage = $"{category}: {message}";
            
            Log.AppendLine(completeLogMessage);

            if (_verbose && ex != null)
            {
                Log.AppendLine(ex.StackTrace);
            }

            if (_writeToLogCat)
            {
                ex = ex ?? new Exception();
                DependencyService.Get<IMessage>().Log(ConvertCategoryToLogLevel(category), completeLogMessage, ex);
            }
            if (_writeToOutput)
            {
                Debug.WriteLine(completeLogMessage, category.ToString());
            }

            if (_writeToConsole)
            {
                Console.WriteLine(completeLogMessage);
            }

            if (_writeToFile)
            {
                //BENDO: Implement logging to file
            }

            if (_writeToToast)
            {
                //BENDO: Implement for UWP 
                DependencyService.Get<IMessage>().ShortAlert(completeLogMessage);
            }
        }
        
        public static LogLevel ConvertCategoryToLogLevel(Category category)
        {
            switch (category)
            {
                case Category.Information:
                    return LogLevel.Info;

                case Category.Error:
                    return LogLevel.Error;

                case Category.Warning:
                    return LogLevel.Warn;

                default:
                    return LogLevel.Debug;
            }
        }

        public static Category ConvertLogLevelToCategory(LogLevel level)
        {
            switch (level.Name.ToLower())
            {
                case "info":
                    return Category.Information;

                case "error":
                    return Category.Error;

                case "warn":
                    return Category.Warning;

                default:
                    return Category.Unknown;
            }
        }
    }

    public enum Category
    {
          Error
        , Warning
        , Information
        , Unknown
    }

}
