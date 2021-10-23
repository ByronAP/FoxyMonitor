using FoxyMonitor.ViewModels;
using System.Windows.Controls;

namespace FoxyMonitor.Views
{
    public partial class AddAccountPage : Page
    {
        public AddAccountPage(AddAccountViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
