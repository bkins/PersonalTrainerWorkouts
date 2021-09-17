using System;

namespace PersonalTrainerWorkouts.Utilities
{
    public static class StringExtensions
    {
        private const string BAD_TIME_FORMAT_MESSAGE = "To convert to a time, the value must be a whole number or in [hh:]mm[:ss] format.";

        public static bool IsNullEmptyOrWhitespace(this string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// "HasValue" means the string is:
        ///     NOT Null,
        /// and NOT Empty,
        /// and NOT Whitespace
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasValue(this string value)
        {
            return ! IsNullEmptyOrWhitespace(value);
        }

        public static TimeSpan ToTime(this string timeAsString)
        {
            //No value, return a new TimeSpan
            if (timeAsString.IsNullEmptyOrWhitespace())
            {
                return new TimeSpan(0
                                  , 0
                                  , 0
                                  , 0);
            }

            //Value is a whole number, return TimeSpan in minutes
            if (int.TryParse(timeAsString
                           , out var number))
            {
                return new TimeSpan(0
                                  , 0
                                  , number
                                  , 0);
            }

            //Value is not in a time format (there is no ':' in string), throw error  
            if (! timeAsString.Contains(":"))
                throw new FormatException(BAD_TIME_FORMAT_MESSAGE);

            var timeParts = timeAsString.Split(':');

            switch (timeParts.Length)
            {
                case 2: //00:00

                    return TryToGetTimeInMinutesAndSeconds(timeParts);

                case 3: //00:00:00

                    return TryToGetTimeInHoursMinutesAndSeconds(timeParts);

                default:

                    throw new FormatException(BAD_TIME_FORMAT_MESSAGE);
            }
        }

        private static TimeSpan TryToGetTimeInHoursMinutesAndSeconds(string[] timeParts)
        {
            TimeSpan time;

            if (int.TryParse(timeParts[0]
                           , out var hours)
             && int.TryParse(timeParts[1]
                           , out var minutes)
             && int.TryParse(timeParts[2]
                           , out var seconds))
            {
                time = new TimeSpan(0
                                  , hours
                                  , minutes
                                  , seconds);
            }

            return time;
        }

        private static TimeSpan TryToGetTimeInMinutesAndSeconds(string[] timeParts)
        {
            TimeSpan time;

            if (int.TryParse(timeParts[0]
                           , out var minutes)
             && int.TryParse(timeParts[1]
                           , out var seconds))
            {
                time = new TimeSpan(0
                                  , 0
                                  , minutes
                                  , seconds);
            }

            return time;
        }

        public static string ToShortForm(this TimeSpan t)
        {
            string shortForm = "";

            if (t.Hours > 0)
            {
                shortForm += $"{t.Hours}:";
            }

            //Add leading zeroes to minutes and seconds
            shortForm += $"{t.Minutes.ToString().PadLeft(2, '0')}:{t.Seconds.ToString().PadLeft(2, '0')}";

            return shortForm;
        }
    }
}
