using FoxyMonitor.ViewModels;
using System.Windows.Controls;

namespace FoxyMonitor.Views
{
    public partial class MainPage : Page
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
