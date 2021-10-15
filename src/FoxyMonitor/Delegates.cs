using FoxyMonitor.Data.Models;

namespace FoxyMonitor
{
    public delegate void SelectedPoolTypeChanged(PoolType poolType);
    public delegate void SelectedPoolNameChanged(string poolName);

    public delegate void ValueChanged(string value);

    public delegate void SelectedThemeModeChanged(ThemeMode themeMode);
    public delegate void SelectedThemeColorChanged(ThemeColor themeColor);
}
