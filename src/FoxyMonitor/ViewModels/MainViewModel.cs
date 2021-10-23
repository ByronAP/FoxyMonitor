using FoxyMonitor.Contracts.Services;
using FoxyMonitor.Contracts.ViewModels;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace FoxyMonitor.ViewModels
{
    public class MainViewModel : ObservableObject, INavigationAware
    {
        public IPostPoolService MyPostPoolService { get; }
        public IAccountService MyAccountService { get; }

        public MainViewModel(IPostPoolService postPoolAccountsService, IAccountService accountService)
        {
            MyPostPoolService = postPoolAccountsService;
            MyAccountService = accountService;
        }

        public void OnNavigatedFrom()
        {
        }

        public void OnNavigatedTo(object parameter)
        {
        }
    }
}
