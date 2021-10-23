using FoxyMonitor.ViewModels;
using System.Windows.Controls;

namespace FoxyMonitor.Views
{
    public partial class SettingsPage : Page
    {
        public SettingsPage(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
