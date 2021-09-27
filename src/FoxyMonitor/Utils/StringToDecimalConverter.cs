using System;
using System.Globalization;
using System.Windows.Data;

namespace FoxyMonitor.Utils
{
    [ValueConversion(typeof(string), typeof(decimal))]
    public class StringToDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return decimal.Parse(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
