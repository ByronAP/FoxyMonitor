using System;
using System.Globalization;
using System.Windows.Data;

namespace FoxyMonitor.Converters
{
    public class PoolNameToPoolDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Error";

            return value.ToString().ToLowerInvariant() switch
            {
                "chia" => "Chia NFT",
                "chia_og" => "Chia OG",
                "chives_og" => "Chives OG",
                "flax_og" => "Flax OG",
                "hddcoin_og" => "HDDCoin OG",
                _ => "FoxyPool",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
