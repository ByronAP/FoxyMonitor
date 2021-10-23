using System;
using System.Globalization;
using System.Windows.Data;

namespace FoxyMonitor.Converters
{
    public class PoolApiNameToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Error";

            return value.ToString().ToLowerInvariant() switch
            {
                "chia" => App.Current.Resources["ChiaNftGlyph"],
                "chia_og" => App.Current.Resources["ChiaGlyph"],
                "chives_og" => App.Current.Resources["ChivesGlyph"],
                "flax_og" => App.Current.Resources["FlaxGlyph"],
                "hddcoin_og" => App.Current.Resources["HddCoinGlyph"],
                _ => App.Current.Resources["FoxyGlyph"],
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
