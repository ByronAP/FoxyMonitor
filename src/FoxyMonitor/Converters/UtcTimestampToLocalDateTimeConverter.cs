using System;
using System.Globalization;
using System.Windows.Data;

namespace FoxyMonitor.Converters
{
    public class UtcTimestampToLocalDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DateTimeOffset.MinValue.ToLocalTime();

            if (value is ulong || value is long)
            {
                var longValue = System.Convert.ToInt64(value);
                return DateTimeOffset.FromUnixTimeMilliseconds(longValue).ToLocalTime();
            }

            return DateTimeOffset.MinValue.ToLocalTime();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
