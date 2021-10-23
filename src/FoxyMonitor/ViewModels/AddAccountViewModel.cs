using FoxyMonitor.Contracts.Services;
using FoxyMonitor.Contracts.ViewModels;
using FoxyMonitor.DbContexts;
using FoxyMonitor.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FoxyMonitor.ViewModels
{
    public class AddAccountViewModel : ObservableObject, INavigationAware
    {
        public IPostPoolService MyPostPoolService { get; }
        public ICommand ValidateLauncerIdCommand => _validateLauncerIdCommand ??= new RelayCommand(OnValidateLauncerIdCommand);
        public ICommand ValidateAndSaveAccountCommand => _validateAndSaveAccountCommand ??= new RelayCommand(OnValidateAndSaveAccountCommand);
        public string LauncherId { get => _launcherId; set => SetProperty(ref _launcherId, value); }
        private string _launcherId = string.Empty;

        public string DisplayName { get => _displayName; set => SetProperty(ref _displayName, value); }
        private string _displayName = string.Empty;

        public string AuthToken { get => _authToken; set => SetProperty(ref _authToken, value.Trim()); }
        private string _authToken = string.Empty;

        private ICommand _validateLauncerIdCommand;
        private ICommand _validateAndSaveAccountCommand;

        private readonly IAccountService _accountService;
        private readonly AppDbContext _appDbContext;

        public AddAccountViewModel(IPostPoolService postPoolAccountsService, IAccountService accountService, AppDbContext appDbContext)
        {
            MyPostPoolService = postPoolAccountsService;
            _accountService = accountService;
            _appDbContext = appDbContext;
        }

        public void OnNavigatedTo(object parameter)
        {
        }

        public void OnNavigatedFrom()
        {
        }

        private async void OnValidateLauncerIdCommand()
        {
            if (string.IsNullOrWhiteSpace(LauncherId))
            {
                MessageBox.Show("Invalid Launcher Id!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (AccountExists())
            {
                MessageBox.Show("Account Already Exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = await _accountService.IsLauncherIdValidAsync(MyPostPoolService.SelectedPostPool.PoolApiName, LauncherId);
            if (result == null)
            {
                MessageBox.Show("Invalid Launcher Id!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (result.Trim().Length > 0)
                {
                    DisplayName = result;
                }
            }
        }

        private async void OnValidateAndSaveAccountCommand()
        {
            if (string.IsNullOrWhiteSpace(LauncherId))
            {
                MessageBox.Show("Invalid Launcher Id!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (AccountExists())
            {
                MessageBox.Show("Account Already Exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var accountResult = await _accountService.GetAccountFromApiAsync(MyPostPoolService.SelectedPostPool.PoolApiName, LauncherId);
            if (accountResult != null && !string.IsNullOrWhiteSpace(accountResult.PayoutAddress))
            {
                if (AuthToken.Trim() != string.Empty)
                {
                    var isAuthTokenValid = await _accountService.IsAuthTokenValidAsync(MyPostPoolService.SelectedPostPool.PoolApiName, LauncherId, AuthToken);
                    if (!isAuthTokenValid)
                    {
                        MessageBox.Show("Invalid Auth Token!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                var newAccount = new Account
                {
                    LauncherId = LauncherId,
                    DisplayName = DisplayName,
                    AuthToken = AuthToken,
                    PoolType = PoolType.POST,
                    PoolName = MyPostPoolService.SelectedPostPoolName.ToLowerInvariant(),
                    LastAcceptedPartialTime = (ulong)accountResult.LastAcceptedPartialAt.ToUnixTimeMilliseconds(),
                    Collateral = accountResult.Collateral == null ? 0 : decimal.Parse(accountResult.Collateral),
                    Difficulty = accountResult.Difficulty,
                    DistributionRatio = accountResult.DistributionRatio ?? "0-100",
                    EstCapacity = accountResult.Ec,
                    PendingBalance = accountResult.Pending == null ? 0 : decimal.Parse(accountResult.Pending),
                    PayoutAddress = accountResult.PayoutAddress ?? "UNKNOWN",
                    PoolPubKey = accountResult.PoolPublicKey ?? "UNKNOWN",
                    Shares = accountResult.Shares,
                    LastBlockWon = 0,
                    LastUpdated = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };

                try
                {
                    var historicalResult = await _accountService.GetAccountHistoricalFromApiAsync(MyPostPoolService.SelectedPostPool.PoolApiName, LauncherId);

                    if (historicalResult != null)
                    {
                        var historicalItemsList = new ObservableCollection<PostAccountHistoricalDbItem>();
                        foreach (var item in historicalResult)
                        {
                            historicalItemsList.Add(PostAccountHistoricalDbItem.FromApiItem(item));
                        }

                        newAccount.PostAccountHistoricalDbItems = historicalItemsList;
                    }
                    else
                    {
                        newAccount.PostAccountHistoricalDbItems = new ObservableCollection<PostAccountHistoricalDbItem>();
                    }
                }
                catch //(Exception ex)
                {
                    //Logger.LogError(ex, "Failed to create new account historical entries, possible api failure.");
                }

                _accountService.AddAccount(newAccount);

                MessageBox.Show("Account Added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                LauncherId = String.Empty;
                AuthToken = String.Empty;
                DisplayName = String.Empty;
            }
            else
            {
                MessageBox.Show("Invalid Launcher Id!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool AccountExists()
        {
            var poolName = MyPostPoolService.SelectedPostPool.PoolApiName.ToString().ToLower();
            return _appDbContext.Accounts.Any(x => x.LauncherId.ToLower() == LauncherId.ToLower() && x.PoolName.ToLower() == poolName);
        }
    }
}
