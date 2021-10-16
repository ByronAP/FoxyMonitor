using System;
using System.Globalization;
using System.Windows.Data;

namespace FoxyMonitor.Utils
{
    [ValueConversion(typeof(string), typeof(string))]
    public class GiBToDisplayStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Error";

#pragma warning disable CS8604 // Possible null reference argument.
            var gibValue = decimal.Parse(value.ToString());
#pragma warning restore CS8604 // Possible null reference argument.
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
