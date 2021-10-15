using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FoxyMonitor.Controls
{
    /// <summary>
    /// Interaction logic for EditSettingsControl.xaml
    /// </summary>
    public partial class EditSettingsControl : UserControl
    {
        private MainAppWindow ParentWindow;

        public EditSettingsControl()
        {
            InitializeComponent();
        }

        private async void DeleteLogs_Click(object sender, RoutedEventArgs e)
        {
            var dialogResult = await DialogManager.ShowMessageAsync(ParentWindow, "Confirm Delete", "Delete all log files?", MessageDialogStyle.AffirmativeAndNegative);

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
                _ = await DialogManager.ShowMessageAsync(ParentWindow, "Complete", $"Deleted {delCount} log archive files.", MessageDialogStyle.Affirmative);
            }
            e.Handled = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ParentWindow = (MainAppWindow)Window.GetWindow(this);
        }

        private void AccountsUpdateInterval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            var value = Convert.ToUInt32(e.NewValue);
            if (value > 15 && value <= 3600)
            {
                Properties.Settings.Default.AccountsUpdateInterval = TimeSpan.FromSeconds(value);
                Properties.Settings.Default.Save();
            }
        }
    }
}
