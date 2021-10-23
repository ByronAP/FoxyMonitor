using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FoxyMonitor.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                if (value == null) return Visibility.Collapsed;

                return Visibility.Visible;
            }

            if (value == null) return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
