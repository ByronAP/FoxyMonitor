using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace FoxyMonitor.Utils
{
    [ValueConversion(typeof(string), typeof(ControlTemplate))]
    public class PoolNameToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Error";

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            switch (value.ToString().ToLowerInvariant())
            {
                case "chia":
                    return App.Current.Resources["ChiaNftGlyph"];
                case "chia_og":
                    return App.Current.Resources["ChiaGlyph"];
                case "chives_og":
                    return App.Current.Resources["ChivesGlyph"];
                case "flax_og":
                    return App.Current.Resources["FlaxGlyph"];
                case "hddcoin_og":
                    return App.Current.Resources["HddCoinGlyph"];
                default:
                    return App.Current.Resources["FoxyGlyph"];
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
