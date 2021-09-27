using System;
using System.Globalization;
using System.Windows.Data;

namespace FoxyMonitor.Utils
{
    [ValueConversion(typeof(string), typeof(string))]
    public class TiBToDisplayStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tibValue = decimal.Parse(value.ToString());
            var pibValue = tibValue / 1024;
            var eibValue = pibValue / 1024;

            if(eibValue > 1) return $"{eibValue:N3} EiB";
            
            if(pibValue >  1) return $"{pibValue:N3} PiB";

            return $"{tibValue:N3} TiB";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
