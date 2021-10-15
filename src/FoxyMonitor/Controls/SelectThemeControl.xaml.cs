using FoxyMonitor.Data.Models;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace FoxyMonitor.Controls
{
    /// <summary>
    /// Interaction logic for SelectThemeControl.xaml
    /// </summary>
    public partial class SelectThemeControl : UserControl
    {
        public static readonly DependencyProperty ThemeModeProperty =
            DependencyProperty.Register("ThemeMode", typeof(ThemeMode),
                typeof(UserInputControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ThemeColorProperty =
            DependencyProperty.Register("ThemeColor", typeof(ThemeColor),
                typeof(UserInputControl), new PropertyMetadata(null));

        public ThemeMode ThemeMode { get => (ThemeMode)GetValue(ThemeModeProperty); set { SetValue(ThemeModeProperty, value); } }
        public ThemeColor ThemeColor { get => (ThemeColor)GetValue(ThemeColorProperty); set { SetValue(ThemeColorProperty, value); } }

        public event SelectedThemeModeChanged OnSelectedThemeModeChanged;
        public event SelectedThemeColorChanged OnSelectedThemeColorChanged;

        private MainAppWindow ParentWindow;

        public SelectThemeControl()
        {
            InitializeComponent();
        }

        private async void ThemeMode_SplitButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeMode_SplitButton == null || ThemeColor_SplitButton == null) return;

            if (ThemeMode_SplitButton.SelectedItem is ThemeMode selectedThemeMode)
            {
                var progressDialog = await ParentWindow.ShowProgressAsync("Please wait...", "Applying Theme", false);
                try
                {
                    ThemeMode = selectedThemeMode;

                    switch (selectedThemeMode)
                    {
                        case ThemeMode.Auto:
                            ThemeColor_SplitButton.IsEnabled = false;
                            break;
                        default:
                            ThemeColor_SplitButton.IsEnabled = true;
                            break;
                    }

                    OnSelectedThemeModeChanged?.Invoke(selectedThemeMode);

                    Properties.Settings.Default.ThemeMode = selectedThemeMode;
                    Properties.Settings.Default.Save();

                    App.SetTheme();
                }
                finally
                {
                    await progressDialog.CloseAsync();
                }
            }
        }

        private async void ThemeColor_SplitButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeMode_SplitButton == null || ThemeColor_SplitButton == null) return;

            if (ThemeColor_SplitButton.SelectedItem is ThemeColor selectedThemeColor)
            {
                var progressDialog = await ParentWindow.ShowProgressAsync("Please wait...", "Applying Theme", false);
                try
                {
                    ThemeColor = selectedThemeColor;

                    switch (selectedThemeColor)
                    {
                        default:
                            break;
                    }

                    OnSelectedThemeColorChanged?.Invoke(selectedThemeColor);

                    Properties.Settings.Default.ThemeColor = selectedThemeColor;
                    Properties.Settings.Default.Save();

                    App.SetTheme();
                }
                finally
                {
                    await progressDialog.CloseAsync();
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ParentWindow = (MainAppWindow)Window.GetWindow(this);

            ThemeMode = Properties.Settings.Default.ThemeMode;
            ThemeColor = Properties.Settings.Default.ThemeColor;

            ThemeMode_SplitButton.SelectedValue = ThemeMode;
            ThemeColor_SplitButton.SelectedValue = ThemeColor;
        }
    }
}
