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
                var rewardPerPiB = ((MainAppWindow)App.Current.MainWindow).MainAppViewModel.SelectedPostPoolInfo?.DailyRewardPerPiB;
                if (rewardPerPiB == null) rewardPerPiB = 0m;
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
