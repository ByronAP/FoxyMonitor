using System;
using System.Globalization;
using System.Windows.Data;

namespace FoxyMonitor.Converters
{
    public class GiBToDisplayStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Error";

            var gibValue = decimal.Parse(value.ToString());
            var tibValue = gibValue / 1024;
            var pibValue = tibValue / 1024;
            var eibValue = pibValue / 1024;

            if (eibValue > 1) return $"{eibValue:N3} EiB";

            if (pibValue > 1) return $"{pibValue:N3} PiB";

            if (tibValue > 1) return $"{tibValue:N3} TiB";

            return $"{gibValue:N3} GiB";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
