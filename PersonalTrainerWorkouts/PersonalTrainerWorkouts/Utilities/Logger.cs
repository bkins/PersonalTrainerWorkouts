using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.Text;
using NLog;
using PersonalTrainerWorkouts.Utilities.Interfaces;

namespace PersonalTrainerWorkouts.Utilities
{
    public static class Logger
    {
        //BENDO: Implement a settings page to handle the setting of these values (or at least the ones that are appropriate)
        public static bool WriteToOutput  { get; set; }
        public static bool WriteToConsole { get; set; }
        public static bool WriteToFile    { get; set; }
        public static bool WriteToToast   { get; set; }
        public static bool WriteToLogCat  { get; set; }
        public static bool Verbose        { get; set; }

        public static StringBuilder Log { get; }

        public static string CompleteLog => Log.ToString();

        static Logger()
        {
            Log = new StringBuilder();

            WriteToOutput  = true;
            WriteToConsole = false;
            WriteToFile    = false;
            WriteToToast   = true;
            WriteToLogCat  = true;
            Verbose        = true;
        }

        public static void WriteLine(string    message
                                   , Category  category
                                   , Exception ex = null)
        {
            var completeLogMessage = $"{category}: {message}";

            Log.AppendLine(completeLogMessage);

            LogVerboseInfo(ex);

            LogToLogCat(category
                      , ex
                      , completeLogMessage);

            LogToOutput(category
                      , completeLogMessage);

            LogToConsole(completeLogMessage);

            LogToFile(completeLogMessage);

            LogToToast(completeLogMessage);
        }

        private static void LogToToast(string completeLogMessage)
        {
            if (WriteToToast)
            {
                //BENDO: Implement for UWP 
                DependencyService.Get<IMessage>()
                                 .ShortAlert(completeLogMessage);
            }
        }

        private static void LogToFile(string completeLogMessage)
        {
            if (WriteToFile)
            {
                //BENDO: Implement logging to file
            }
        }

        private static void LogToConsole(string completeLogMessage)
        {
            if (WriteToConsole)
            {
                Console.WriteLine(completeLogMessage);
            }
        }

        private static void LogToOutput(Category category
                                      , string   completeLogMessage)
        {
            if (WriteToOutput)
            {
                Debug.WriteLine(completeLogMessage
                              , category.ToString());
            }
        }

        private static void LogToLogCat(Category  category
                                      , Exception ex
                                      , string    completeLogMessage)
        {
            if (WriteToLogCat)
            {
                ex = ex ?? new Exception();

                DependencyService.Get<IMessage>()
                                 .Log(ConvertCategoryToLogLevel(category)
                                    , completeLogMessage
                                    , ex);
            }
        }

        private static void LogVerboseInfo(Exception ex)
        {
            if (Verbose && ex != null)
            {
                Log.AppendLine(ex.StackTrace);
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

                case Category.Unknown:
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
