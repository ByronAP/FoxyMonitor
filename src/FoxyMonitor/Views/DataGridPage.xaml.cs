using FoxyMonitor.ViewModels;
using System.Windows.Controls;

namespace FoxyMonitor.Views
{
    public partial class DataGridPage : Page
    {
        public DataGridPage(DataGridViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
