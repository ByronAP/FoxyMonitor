using System;
using System.Globalization;
using System.Windows.Data;

namespace FoxyMonitor.Utils
{
    [ValueConversion(typeof(DateTimeOffset), typeof(DateTimeOffset))]
    public class UtcDateTimeToLocalDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTimeOffset)value).ToLocalTime();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
