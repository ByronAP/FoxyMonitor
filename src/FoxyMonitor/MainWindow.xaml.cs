using FoxyMonitor.Data.Models;
using FoxyMonitor.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Logging;
using SplotControl.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace FoxyMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        internal readonly ILogger<MainWindow> Logger;
        internal readonly AppViewModel MainAppViewModel;

        public MainWindow(AppViewModel appViewModel, ILogger<MainWindow> logger)
        {
            Logger = logger;
            MainAppViewModel = appViewModel;

            DataContext = MainAppViewModel;
            InitializeComponent();

            var foregroundBrush = Foreground as SolidColorBrush;
            appViewModel.ChartBrush = Foreground;
            appViewModel.GridOptions = new GridOptions
            {
                GridBrush = new SolidColorBrush(Color.FromArgb(40, foregroundBrush.Color.R, foregroundBrush.Color.G, foregroundBrush.Color.B)),
                GridLineWidth = 2,
                FontSize = 8,
                GridLineCount = 6
            };
        }

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            AddAccountFlyout.IsOpen = true;
            e.Handled = true;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            _ = Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsFlyout.IsOpen = true;
            e.Handled = true;
        }

        private void OpenAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutFlyout.IsOpen = true;
            e.Handled = true;
        }

        private void OpenAccounts_Click(object sender, RoutedEventArgs e)
        {
            AccountsFlyout.IsOpen = true;
            e.Handled = true;
        }

        private async void DeleteLogs_Click(object sender, RoutedEventArgs e)
        {
            var dialogResult = await DialogManager.ShowMessageAsync(this, "Confirm Delete", "Delete all log files?", MessageDialogStyle.AffirmativeAndNegative);

            if (dialogResult == MessageDialogResult.Affirmative)
            {
                var logFiles = Directory.GetFiles(Utils.IOUtils.GetLoggingDirectory());
                var delCount = 0;
                foreach (var logFile in logFiles)
                {
                    try
                    {
                        File.Delete(logFile);
                        delCount++;
                    }
                    catch
                    {
                        // ignore because we can not and will not delete the current log file
                    }
                }
                _ = await DialogManager.ShowMessageAsync(this, "Complete", $"Deleted {delCount} log archive files.", MessageDialogStyle.Affirmative);
            }
            e.Handled = true;
        }

        private async void Accounts_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var opacityAnimationIn = new DoubleAnimation(1, 0.2, TimeSpan.FromSeconds(0.2));
            var opacityAnimationOut = new DoubleAnimation(0.2, 1, TimeSpan.FromSeconds(0.2));

            if (Content_Grid != null)
            {
                Content_Grid.BeginAnimation(OpacityProperty, opacityAnimationIn);
            }

            await Task.Delay(TimeSpan.FromSeconds(0.1));

            MainAppViewModel.SelectedAccountId = (Accounts_ListView.SelectedItem as Account).Id;

            AccountsFlyout.IsOpen = false;

            if (SharesPlotControl != null)
            {
                SharesPlotControl.GridOptions = MainAppViewModel.GridOptions;
                SharesPlotControl.ColumnSeries = MainAppViewModel.SelectedAccountSharesSeries;
            }


            if (EcPlotControl != null)
            {
                EcPlotControl.GridOptions = MainAppViewModel.GridOptions;
                EcPlotControl.ColumnSeries = MainAppViewModel.SelectedAccountEstCapacitySeries;
            }

            await Task.Delay(TimeSpan.FromSeconds(0.1));

            if (Content_Grid != null)
            {
                Content_Grid.BeginAnimation(OpacityProperty, opacityAnimationOut);
            }
        }

        public void BringToForeground()
        {
            if (WindowState == WindowState.Minimized || Visibility == Visibility.Hidden)
            {
                Show();
                WindowState = WindowState.Normal;
            }

            _ = Activate();
            Topmost = true;
            Topmost = false;
            _ = Focus();
        }
    }
}