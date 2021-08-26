using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalTrainerWorkouts.Utilities
{
    public static class StringExtensions
    {
        public static bool IsNullEmptyOrWhitespace(this string value)
        {
            return string.IsNullOrEmpty(value) 
                || string.IsNullOrWhiteSpace(value);
        }
        /// <summary>
        /// "HasValue" means the string is NOT Null, and NOT Empty, and NOT Whitespace
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasValue(this string value)
        {
            return ! IsNullEmptyOrWhitespace(value);
        }
        public static TimeSpan ToTime(this string timeAsString)
        {
            if (timeAsString == null)
            {
                return new TimeSpan(0, 0, 0, 0);
            }

            //string passed in is a whole number, convert to 1:00 (one minute)
            if (int.TryParse(timeAsString, out var number))
            {
                return new TimeSpan(0
                                  , 0
                                  , number
                                  , 0);
            }

                        
            if (timeAsString.Contains(":"))
            {
                var timeParts = timeAsString.Split(':');

                switch (timeParts.Length)
                {
                    case 2:
                        
                        if (int.TryParse(timeParts[0], out var minutes)
                         && int.TryParse(timeParts[1], out var seconds))
                        {
                            return new TimeSpan(0
                                              , 0
                                              , minutes
                                              , seconds);
                        }
                        
                        break;

                    case 3:

                        var hoursAsString   = timeParts[0];
                        var minutesAsString = timeParts[1];
                        var secondsAsString = timeParts[2];

                        if (int.TryParse(hoursAsString,   out var hours)
                         && int.TryParse(minutesAsString, out minutes)
                         && int.TryParse(secondsAsString, out seconds))
                        {
                            return new TimeSpan(0
                                              , hours
                                              , minutes
                                              , seconds);
                        }

                        break;

                    default:

                        throw new FormatException("To convert to a time, the value must be a whole number or in mm:ss format.");
                }
            }

            throw new FormatException("To convert to a time, the value must be a whole number or in mm:ss format.");
            //return new TimeSpan();

        }

        public static bool IsStringInStringMoreThanOnce(this string stringToSearchThrough, string valueToSearch)
        {
            var firstIndex = stringToSearchThrough.IndexOf(valueToSearch
                                                         , StringComparison.Ordinal);

            var result = firstIndex != stringToSearchThrough.LastIndexOf(valueToSearch
                                                                       , StringComparison.Ordinal) 
                      && firstIndex != -1;

            return result;
        }

        public static bool IsCharInStringExactlyTwice(this string stringToSearchThrough
                                                      , char      valueToSearch)
        {
            var test = stringToSearchThrough.Split(valueToSearch);

            return test.Length == 3;

        }
        public static string ToShortForm(this TimeSpan t)
        {
            string shortForm = "";
            if (t.Hours > 0)
            {
                shortForm += $"{t.Hours}:";
            }

            shortForm += $"{t.Minutes.ToString().PadLeft(2, '0')}:{t.Seconds.ToString().PadLeft(2, '0')}";
            
            //if (t.Minutes > 0)
            //{
            //    shortForm += $"{t.Minutes}";
            //}
            //if (t.Seconds > 0)
            //{
            //    shortForm += $":{t.Seconds}";
            //}
            return shortForm;
        }
        
    }
}
