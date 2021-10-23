using System;
using System.Globalization;
using System.Windows.Data;

namespace FoxyMonitor.Converters
{
    public class PoolApiNameToCurrencyCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            var valueStr = value as string;

            return valueStr.ToLowerInvariant() switch
            {
                "chives_og" => "XCC",
                "hddcoin_og" => "HDD",
                "flax_og" => "XFX",
                _ => "XCH",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
