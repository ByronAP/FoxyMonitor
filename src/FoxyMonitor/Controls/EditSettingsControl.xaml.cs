using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FoxyMonitor.Controls
{
    /// <summary>
    /// Interaction logic for EditSettingsControl.xaml
    /// </summary>
    public partial class EditSettingsControl : UserControl
    {
        private MainAppWindow? ParentWindow;

        public EditSettingsControl()
        {
            InitializeComponent();
        }

        private async void CheckForUpdates(object sender, RoutedEventArgs e)
        {
            //TODO: Check for updates
            await Task.CompletedTask;
        }

        private async void DeleteLogs(object sender, RoutedEventArgs e)
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

            if (Properties.Settings.Default.LogToFile)
            {
                File_Logging_Radio.IsChecked = true;
                Console_Logging_Radio.IsChecked = false;
            }
            else
            {
                File_Logging_Radio.IsChecked = false;
                Console_Logging_Radio.IsChecked = true;
            }

            Auto_Updates_Check.IsChecked = Properties.Settings.Default.AutoCheckForUpdates;
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

        private void Logging_Changed(object sender, RoutedEventArgs e)
        {
            var radio = e.OriginalSource as RadioButton;
            if (radio != null && radio.Name == nameof(File_Logging_Radio))
            {
                Properties.Settings.Default.LogToFile = true;
                Properties.Settings.Default.Save();
            }
            else if (radio != null && radio.Name == nameof(Console_Logging_Radio))
            {
                Properties.Settings.Default.LogToFile = false;
                Properties.Settings.Default.ShowConsole = true;
                Properties.Settings.Default.Save();
            }
        }

        private void AutoUpdate_CheckChanged(object sender, RoutedEventArgs e)
        {
            var check = e.OriginalSource as CheckBox;

            if (check == null || check.IsChecked == null) return;

            Properties.Settings.Default.AutoCheckForUpdates = (bool)check.IsChecked;
            Properties.Settings.Default.Save();
        }
    }
}
