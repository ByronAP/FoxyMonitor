using System;
using System.Globalization;
using System.Windows.Data;

namespace FoxyMonitor.Converters
{
    public class TimeToAgoStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Error";

            var dateTime = DateTimeOffset.MinValue;

            if (value is DateTimeOffset offsetValue) dateTime = offsetValue;

            if (value is DateTime timeValue) dateTime = timeValue;

            if (value is long longValue) dateTime = DateTimeOffset.FromUnixTimeMilliseconds(longValue);

            if (value is ulong ulongValue) dateTime = DateTimeOffset.FromUnixTimeMilliseconds(System.Convert.ToInt64(ulongValue));

            string result;

            var timeSpan = DateTimeOffset.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("about {0} seconds ago", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    string.Format("about {0} minutes ago", timeSpan.Minutes) :
                    "about a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    string.Format("about {0} hours ago", timeSpan.Hours) :
                    "about an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    string.Format("about {0} days ago", timeSpan.Days) :
                    "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    string.Format("about {0} months ago", timeSpan.Days / 30) :
                    "about a month ago";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    string.Format("about {0} years ago", timeSpan.Days / 365) :
                    "about a year ago";
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
