using System;
using System.Globalization;
using System.Windows.Data;

namespace FoxyMonitor.Converters
{
    public class TiBToDisplayStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Error";

            if (value is string)
            {
                if (string.IsNullOrEmpty(value as string)) value = "0";
            }

            var tibValue = decimal.Parse(value.ToString());
            var pibValue = tibValue / 1024;
            var eibValue = pibValue / 1024;

            if (eibValue > 1) return $"{eibValue:N3} EiB";

            if (pibValue > 1) return $"{pibValue:N3} PiB";

            return $"{tibValue:N3} TiB";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
