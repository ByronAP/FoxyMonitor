using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace FoxyMonitor.Converters
{
    public class PoolAndAccountToDisplayNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return "Error";
            if (parameter == null) parameter = " ";

            if (values.Length > 2)
            {
                var stringBuilder = new StringBuilder();
                foreach (var item in values)
                    stringBuilder.Append(item.ToString()).Append(parameter.ToString());
                return stringBuilder.ToString().Trim();
            }

            if (values.Length == 2)
                return $"{values[0]}{parameter}{values[1]}";

            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
