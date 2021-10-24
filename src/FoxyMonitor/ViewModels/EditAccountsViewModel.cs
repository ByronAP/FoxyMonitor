using FoxyMonitor.Contracts.ViewModels;
using FoxyMonitor.DbContexts;
using FoxyMonitor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace FoxyMonitor.ViewModels
{
    public class EditAccountsViewModel : ObservableObject, INavigationAware
    {
        private readonly AppDbContext _appDbContext;

        public ObservableCollection<Account> Source { get; set; }

        public EditAccountsViewModel(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void OnNavigatedTo(object parameter)
        {
            _appDbContext.Accounts.Load();
            Source = _appDbContext.Accounts.Local.ToObservableCollection();
        }

        public void OnNavigatedFrom()
        {
        }
    }
}
