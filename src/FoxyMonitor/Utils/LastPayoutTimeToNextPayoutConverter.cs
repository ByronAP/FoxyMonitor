using System;
using System.Globalization;
using System.Windows.Data;

namespace FoxyMonitor.Utils
{
    [ValueConversion(typeof(DateTimeOffset), typeof(string))]
    public class LastPayoutTimeToNextPayoutConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var lastPayout = (DateTimeOffset)value;
            var nextPayout = lastPayout.AddHours(24);
            var timeSpan = (nextPayout - DateTimeOffset.UtcNow).Duration();
            string result;

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("about {0} seconds", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    string.Format("about {0} minutes", timeSpan.Minutes) :
                    "about a minute";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    string.Format("about {0} hours", timeSpan.Hours) :
                    "about an hour";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    string.Format("about {0} days", timeSpan.Days) :
                    "tomorrow";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    string.Format("about {0} months", timeSpan.Days / 30) :
                    "about a month";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    string.Format("about {0} years", timeSpan.Days / 365) :
                    "about a year";
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
