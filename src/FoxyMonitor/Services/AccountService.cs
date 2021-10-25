using FoxyMonitor.Contracts.Services;
using FoxyMonitor.DbContexts;
using FoxyMonitor.Models;
using FoxyPoolApi;
using FoxyPoolApi.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FoxyMonitor.Services
{
    public class AccountService : ObservableObject, IAccountService
    {
        public ObservableCollection<string> AccountNames { get; private set; } = new ObservableCollection<string>();

        public ObservableCollection<Account> Accounts { get => _appDbContext.Accounts.Local.ToObservableCollection(); }

        public string SelectedAccountName { get => _selectedAccount.DisplayName; }

        public Account SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                if (value == null) return;

                OnPropertyChanging(nameof(SelectedAccountName));
                OnPropertyChanging(nameof(SelectedAccountEstDailyReward));
                OnPropertyChanging(nameof(SelectedAccountPostPoolInfo));
                SetProperty(ref _selectedAccount, value);
                OnPropertyChanged(nameof(SelectedAccountPostPoolInfo));
                OnPropertyChanged(nameof(SelectedAccountEstDailyReward));
                OnPropertyChanged(nameof(SelectedAccountName));
            }
        }

        public PostPoolInfo SelectedAccountPostPoolInfo
        {
            get
            {
                if (SelectedAccount == null) return new PostPoolInfo();

                var postPool = (PostPool)Enum.Parse(typeof(PostPool), SelectedAccount.PoolName, true);
                var poolInfo = _appDbContext.PostPools.FirstOrDefault(
                  x => x.PoolApiName == postPool);

                if (poolInfo != null) return poolInfo;

                return new PostPoolInfo();
            }
        }

        public decimal SelectedAccountEstDailyReward
        {
            get
            {
                if (SelectedAccount == null) return 0m;

                try
                {
                    var rewardPerPiB = SelectedAccountPostPoolInfo.DailyRewardPerPiB;
                    var estCapacity = SelectedAccount.EstCapacity;
                    return estCapacity / 1024 / 1024 * rewardPerPiB;
                }
                catch
                {
                    return 0m;
                }
            }
        }

        private Account _selectedAccount;

        private readonly AppDbContext _appDbContext;

        public AccountService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _appDbContext.ChangeTracker.StateChanged += ChangeTracker_StateChanged;
            _appDbContext.Accounts.Load();
        }

        private void ChangeTracker_StateChanged(object sender, EntityStateChangedEventArgs e)
        {
            if (e == null || e.Entry.Entity.GetType() != typeof(Account)) return;

            var account = e.Entry.Entity as Account;
            if (_selectedAccount != null && _selectedAccount.Id == account.Id)
            {
                _selectedAccount = account;
            }

        }

        public bool AddAccount(Account account)
        {
            _appDbContext.Accounts.Add(account);

            var count = _appDbContext.SaveChanges();

            if (count > 0 && SelectedAccount == null)
            {
                SelectedAccount = account;
            }

            return count > 0;
        }

        public async Task<bool> IsAuthTokenValidAsync(PostPool postPool, string launcherId, string authToken)
        {
            try
            {
                using var postApiClient = new PostApiClient(postPool);
                var accountResult = await postApiClient.GetAccountAsync(launcherId);
                if (accountResult != null && !string.IsNullOrWhiteSpace(accountResult.PayoutAddress))
                {
                    if (authToken.Trim() != string.Empty)
                    {
                        if (string.IsNullOrEmpty(accountResult.Name))
                            accountResult.Name = new Random().Next(1000).ToString();

                        var updateAccountNameResult = await postApiClient.UpdateAccountNameAsync(accountResult.Name, launcherId, authToken);
                        if (updateAccountNameResult != null)
                        {
                            return updateAccountNameResult.StatusCode == System.Net.HttpStatusCode.NoContent;
                        }
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> IsLauncherIdValidAsync(PostPool postPool, string launcherId)
        {
            try
            {
                using var postApiClient = new PostApiClient(postPool);
                var accountResult = await postApiClient.GetAccountAsync(launcherId);
                if (accountResult != null && !string.IsNullOrWhiteSpace(accountResult.PayoutAddress))
                {
                    if (!string.IsNullOrEmpty(accountResult.Name))
                    {
                        return accountResult.Name;
                    }
                    return "";
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public void RemoveAccount(uint accountId)
        {
            var account = _appDbContext.Accounts.Find(accountId);
            if (account == null) _appDbContext.Accounts.Remove(account);
            _appDbContext.SaveChanges();
        }

        public void RemoveAccount(Account account)
        {
            _appDbContext.Accounts.Remove(account);
            _appDbContext.SaveChanges();
        }

        public async Task<PostAccountResponse> GetAccountFromApiAsync(PostPool postPool, string launcherId)
        {
            try
            {
                using var postApiClient = new PostApiClient(postPool);

                return await postApiClient.GetAccountAsync(launcherId);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<PostAccountHistoricalItem>> GetAccountHistoricalFromApiAsync(PostPool postPool, string launcherId)
        {
            try
            {
                using var postApiClient = new PostApiClient(postPool);

                return await postApiClient.GetAccountHistoricalAsync(launcherId);
            }
            catch
            {
                return null;
            }
        }
    }
}
