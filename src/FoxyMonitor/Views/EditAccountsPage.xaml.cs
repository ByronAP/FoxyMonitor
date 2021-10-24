using FoxyMonitor.ViewModels;
using System.Windows.Controls;

namespace FoxyMonitor.Views
{
    public partial class EditAccountsPage : Page
    {
        public EditAccountsPage(EditAccountsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
