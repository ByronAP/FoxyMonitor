using FoxyMonitor.Models;
using System.Collections.ObjectModel;

namespace FoxyMonitor.Contracts.Services
{
    public interface IPostPoolService
    {
        ObservableCollection<string> PostPoolNames { get; }

        ObservableCollection<PostPoolInfo> PostPools { get; }

        string SelectedPostPoolName { get; set; }

        PostPoolInfo SelectedPostPool { get; set; }
    }
}
