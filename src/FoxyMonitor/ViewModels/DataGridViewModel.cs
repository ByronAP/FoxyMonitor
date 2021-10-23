using FoxyMonitor.Contracts.ViewModels;
using FoxyMonitor.DbContexts;
using FoxyMonitor.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace FoxyMonitor.ViewModels
{
    public class DataGridViewModel : ObservableObject, INavigationAware
    {
        private readonly AppDbContext _sampleDataService;

        public ObservableCollection<Account> Source { get; } = new ObservableCollection<Account>();

        public DataGridViewModel(AppDbContext sampleDataService)
        {
            _sampleDataService = sampleDataService;
        }

        public void OnNavigatedTo(object parameter)
        {
            Source.Clear();

            // Replace this with your actual data
            var data = _sampleDataService.Accounts.Where(x => x.Id >= 0);

            foreach (var item in data)
            {
                Source.Add(item);
            }
        }

        public void OnNavigatedFrom()
        {
        }
    }
}
