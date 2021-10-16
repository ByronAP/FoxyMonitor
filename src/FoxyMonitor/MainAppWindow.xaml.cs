using FoxyMonitor.Data.Models;
using FoxyMonitor.ViewModels;
using MahApps.Metro.Controls;
using Microsoft.Extensions.Logging;
using SplotControl.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace FoxyMonitor
{
    public partial class MainAppWindow : MetroWindow
    {
        internal readonly ILogger<MainAppWindow> Logger;
        internal readonly AppViewModel MainAppViewModel;

        public MainAppWindow(AppViewModel appViewModel, ILogger<MainAppWindow> logger)
        {
            Logger = logger;
            MainAppViewModel = appViewModel;

            DataContext = MainAppViewModel;
            InitializeComponent();

            appViewModel.ChartBrush = EC_Chart_Tile.Foreground;
            appViewModel.GridOptions = new GridOptions
            {
                GridBrush = new SolidColorBrush(Color.FromArgb(40, 200, 200, 200)),
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

        internal void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
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

        private async void Accounts_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var opacityAnimationIn = new DoubleAnimation(1, 0.2, TimeSpan.FromSeconds(0.2));
            var opacityAnimationOut = new DoubleAnimation(0.2, 1, TimeSpan.FromSeconds(0.2));

            if (Content_Grid != null)
            {
                Content_Grid.BeginAnimation(OpacityProperty, opacityAnimationIn);
            }

            await Task.Delay(TimeSpan.FromSeconds(0.1));

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            MainAppViewModel.SelectedAccountId = (Accounts_ListView.SelectedItem as Account).Id;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            AccountsFlyout.IsOpen = false;

            if (SharesPlotControl != null)
            {
                SharesPlotControl.GridOptions = MainAppViewModel.GridOptions;
#pragma warning disable CS8601 // Possible null reference assignment.
                SharesPlotControl.ColumnSeries = MainAppViewModel.SelectedAccountSharesSeries;
#pragma warning restore CS8601 // Possible null reference assignment.
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

        private void metroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Properties.Settings.Default.HideOnExit)
            {
                Hide();
                e.Cancel = true;
            }
        }

        private async void Exit_Click(object sender, RoutedEventArgs e)
        {
            // this helps ensure the notifyicon is gone before exiting the app
            AppNotifyIcon.Visibility = Visibility.Collapsed;
            await Task.Delay(500);
            Application.Current.Shutdown(0);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
            e.Handled = true;
        }
    }
}