using ControlzEx.Theming;
using FoxyMonitor.Contracts.Services;
using FoxyMonitor.Models;
using MahApps.Metro.Theming;
using System;
using System.Windows;

namespace FoxyMonitor.Services
{
    public class ThemeSelectorService : IThemeSelectorService
    {
        private const string HcDarkTheme = "pack://application:,,,/Styles/Themes/HC.Dark.Blue.xaml";
        private const string HcLightTheme = "pack://application:,,,/Styles/Themes/HC.Light.Blue.xaml";
        private readonly ApplicationPropertiesService _appPropertiesService;

        public ThemeSelectorService(IApplicationPropertiesService appPropertiesService)
        {
            _appPropertiesService = (ApplicationPropertiesService)appPropertiesService;
        }

        public void InitializeTheme()
        {
            ThemeManager.Current.AddLibraryTheme(new LibraryTheme(new Uri(HcDarkTheme), MahAppsLibraryThemeProvider.DefaultInstance));
            ThemeManager.Current.AddLibraryTheme(new LibraryTheme(new Uri(HcLightTheme), MahAppsLibraryThemeProvider.DefaultInstance));

            var theme = GetCurrentTheme();
            SetTheme(theme);
        }

        public void SetTheme(AppTheme theme)
        {
            if (theme == AppTheme.Default)
            {
                ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
                ThemeManager.Current.SyncTheme();
            }
            else
            {
                ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithHighContrast;
                ThemeManager.Current.SyncTheme();
                ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.Blue", SystemParameters.HighContrast);
            }

            _appPropertiesService.SetProperty("Theme", theme.ToString());
        }

        public AppTheme GetCurrentTheme()
        {
            if (_appPropertiesService.Contains("Theme"))
            {
                var themeName = _appPropertiesService.GetProperty<string>("Theme");
                if (Enum.TryParse(themeName, out AppTheme theme)) return theme;
            }

            return AppTheme.Default;
        }
    }
}
