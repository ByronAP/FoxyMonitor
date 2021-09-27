using System;
using System.Globalization;
using System.Windows.Data;

namespace FoxyMonitor.Utils
{
    [ValueConversion(typeof(decimal), typeof(decimal))]
    public class CapacityToDailyRewardConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var rewardPerPiB = ((MainWindow)App.Current.MainWindow).MainAppViewModel.SelectedPostPoolInfo?.DailyRewardPerPiB;
                return (decimal)value / 1024 / 1024 * rewardPerPiB;
            }
            catch
            {
                return (decimal)0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
