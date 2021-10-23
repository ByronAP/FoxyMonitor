using FoxyMonitor.Models;
using FoxyPoolApi;
using FoxyPoolApi.Responses;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FoxyMonitor.Contracts.Services
{
    public interface IAccountService
    {
        ObservableCollection<string> AccountNames { get; }

        ObservableCollection<Account> Accounts { get; }

        string SelectedAccountName { get; }

        Account SelectedAccount { get; set; }

        PostPoolInfo SelectedAccountPostPoolInfo { get; }

        decimal SelectedAccountEstDailyReward { get; }

        Task<bool> IsAuthTokenValidAsync(PostPool postPool, string launcherId, string authToken);

        public void RemoveAccount(uint accountId);

        public void RemoveAccount(Account account);

        public bool AddAccount(Account account);

        Task<PostAccountResponse> GetAccountFromApiAsync(PostPool postPool, string launcherId);

        Task<List<PostAccountHistoricalItem>> GetAccountHistoricalFromApiAsync(PostPool postPool, string launcherId);

#nullable enable
        Task<string?> IsLauncherIdValidAsync(PostPool postPool, string launcherId);
    }
}
