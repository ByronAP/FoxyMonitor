using FoxyMonitor.Contracts.Services;
using FoxyMonitor.DbContexts;
using FoxyMonitor.Helpers;
using FoxyMonitor.Models;
using FoxyPoolApi;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FoxyMonitor.Services
{
    public class AccountUpdaterService : IHostedService
    {
        private const string AccountsUpdateIntervalPropertyKeyName = "AccountsUpdateInterval";
        private readonly ApplicationPropertiesService _appPropertiesService;
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<AccountUpdaterService> _logger;
        private readonly SemaphoreSlim _semaphore;
        private readonly IPostChainExplorerService _postChainExplorerService;
        private readonly IToastNotificationsService _notificationsService;
        private Timer _timer;
        private int _executionCount;
        private TimeSpan _interval;

        public AccountUpdaterService(IApplicationPropertiesService appPropertiesService, AppDbContext appDbContext, ILogger<AccountUpdaterService> logger, IPostChainExplorerService postChainExplorerService, IToastNotificationsService toastNotificationsService)
        {
            _appPropertiesService = (ApplicationPropertiesService)appPropertiesService;
            _appDbContext = appDbContext;
            _logger = logger;
            _postChainExplorerService = postChainExplorerService;
            _notificationsService = toastNotificationsService;
            _semaphore = new SemaphoreSlim(1);

            if (appPropertiesService.Contains(AccountsUpdateIntervalPropertyKeyName))
            {
                _interval = TimeSpan.FromSeconds(appPropertiesService.GetProperty<uint>(AccountsUpdateIntervalPropertyKeyName));
            }
            else
            {
                _interval = TimeSpan.FromSeconds(30);
            }

            _appPropertiesService.PropertyChanged += ApplicationPropertiesService_PropertyChanged;
        }

        private void ApplicationPropertiesService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_timer == null || e.PropertyName != AccountsUpdateIntervalPropertyKeyName) return;

            var newInterval = TimeSpan.FromSeconds(_appPropertiesService.GetProperty<uint>(AccountsUpdateIntervalPropertyKeyName));

            if (_interval != newInterval)
            {
                _timer.Change(newInterval, newInterval);

                _interval = newInterval;

                _logger.LogInformation("Account updater interval changed to {NewInterval}", newInterval);
            }

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Account updater service running.");

            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(6), _interval);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Account updater service is stopping.");

            _ = (_timer?.Change(Timeout.Infinite, 0));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                // we use this semaphore to make sure even if a run takes a long time we cant overlap
                await _semaphore.WaitAsync();

                foreach (var account in _appDbContext.Accounts)
                {
                    if (account.PoolType == PoolType.POST)
                    {
                        try
                        {
                            using var postApiClient = new PostApiClient((PostPool)Enum.Parse(typeof(PostPool), account.PoolName, true));
                            var accountData = await postApiClient.GetAccountAsync(account.LauncherId);
                            if (accountData != null)
                            {
                                var pool = _appDbContext.PostPools.FirstOrDefault(x => x.PoolApiName.Equals(Enum.Parse(typeof(PostPool), account.PoolName, true)));
                                var poolName = pool == null ? account.PoolName : pool.PoolName.Replace("Foxy-Pool ", "");

                                if (account.PayoutAddress != null)
                                {
                                    try
                                    {
                                        var newBalance = PostPoolBalanceConverter.ConvertBalance(pool.PoolApiName, await _postChainExplorerService.GetAddressBalanceAsync(pool.PoolApiName, account.PayoutAddress));
                                        if (newBalance != account.PayoutAddressBalance) account.PayoutAddressBalance = newBalance;
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, "Account updater failed to retrieve account balance from chain api.");
                                    }
                                }

                                if (accountData.PoolPublicKey != null && account.PoolPubKey != accountData.PoolPublicKey)
                                {
                                    var hasExistingAlert = _appDbContext.Alerts.Where(x => x.AccountId == account.Id && x.AlertType == AlertType.PoolPubKeyChange).Any();
                                    if (!hasExistingAlert)
                                    {
                                        var newAlert = new Alert
                                        {
                                            AlertType = AlertType.PoolPubKeyChange,
                                            AccountId = account.Id,
                                            Created = Convert.ToUInt64(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
                                            Level = LogLevel.Critical,
                                            Title = $"{poolName} Pool Public Key Changed",
                                            Message = $"The public pool key for {poolName} {account.DisplayName} does not match the local record. {account.PoolPubKey} changed to {accountData.PoolPublicKey}. Plesase verify this change with the pool operator."
                                        };

                                        App.Current.Dispatcher.Invoke(() => _appDbContext.Alerts.Add(newAlert));

                                        var toastTag = $"{newAlert.AlertType}-{newAlert.AccountId}";
                                        _notificationsService.ShowToastNotification(toastTag, newAlert.Title, newAlert.Message, ToastAction.ShowAccount, newAlert.AccountId.ToString());
                                    }

                                    account.PoolPubKey = accountData.PoolPublicKey;
                                }

                                if (accountData.PayoutAddress != null && account.PayoutAddress != accountData.PayoutAddress)
                                {
                                    var hasExistingAlert = _appDbContext.Alerts.Where(x => x.AccountId == account.Id && x.AlertType == AlertType.PayoutAddressChange).Any();
                                    if (!hasExistingAlert)
                                    {
                                        var newAlert = new Alert
                                        {
                                            AlertType = AlertType.PayoutAddressChange,
                                            AccountId = account.Id,
                                            Created = Convert.ToUInt64(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
                                            Level = LogLevel.Critical,
                                            Title = $"{poolName} Payout Address Changed",
                                            Message = $"The payout address for {poolName} {account.DisplayName} changed. {account.PayoutAddress} changed to {accountData.PayoutAddress}. Plesase verify you made this change."
                                        };

                                        App.Current.Dispatcher.Invoke(() => _appDbContext.Alerts.Add(newAlert));

                                        var toastTag = $"{newAlert.AlertType}-{newAlert.AccountId}";
                                        _notificationsService.ShowToastNotification(toastTag, newAlert.Title, newAlert.Message, ToastAction.ShowAccount, newAlert.AccountId.ToString());
                                    }

                                    account.PayoutAddress = accountData.PayoutAddress;
                                }

                                if (account.Shares != accountData.Shares)
                                {
                                    account.Shares = accountData.Shares;
                                }

                                if (account.LastAcceptedPartialTime != Convert.ToUInt64(accountData.LastAcceptedPartialAt.ToUnixTimeMilliseconds()))
                                {
                                    var hasExistingAlert = _appDbContext.Alerts.Where(x => x.AccountId == account.Id && x.AlertType == AlertType.SlowPartials).Any();
                                    if (hasExistingAlert)
                                    {
                                        var alert = _appDbContext.Alerts.FirstOrDefault(x => x.AccountId == account.Id && x.AlertType == AlertType.SlowPartials);
                                        if (alert != null)
                                        {
                                            App.Current.Dispatcher.Invoke(() => _appDbContext.Alerts.Remove(alert));
                                        }
                                    }
                                    account.LastAcceptedPartialTime = Convert.ToUInt64(accountData.LastAcceptedPartialAt.ToUnixTimeMilliseconds());
                                }
                                else if (account.AlertOnSlowPartials)
                                {
                                    var diff = DateTimeOffset.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(account.LastAcceptedPartialTime));
                                    if (diff >= account.MaxPartialTime)
                                    {
                                        var hasExistingAlert = _appDbContext.Alerts.Where(x => x.AccountId == account.Id && x.AlertType == AlertType.SlowPartials).Any();
                                        if (!hasExistingAlert)
                                        {
                                            var newAlert = new Alert
                                            {
                                                AlertType = AlertType.SlowPartials,
                                                AccountId = account.Id,
                                                Created = Convert.ToUInt64(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
                                                Level = LogLevel.Critical,
                                                Title = $"{poolName} Slow Partials",
                                                Message = $"{poolName} {account.DisplayName} has not had an accepted partial in {diff.TotalMinutes:F2} minutes."
                                            };

                                            App.Current.Dispatcher.Invoke(() => _appDbContext.Alerts.Add(newAlert));

                                            var toastTag = $"{newAlert.AlertType}-{newAlert.AccountId}";
                                            _notificationsService.ShowToastNotification(toastTag, newAlert.Title, newAlert.Message, ToastAction.ShowAccount, newAlert.AccountId.ToString());
                                        }
                                    }
                                }

                                if (account.EstCapacity != accountData.Ec)
                                {
                                    account.EstCapacity = accountData.Ec;
                                }

                                if (accountData.DistributionRatio != null && account.DistributionRatio != accountData.DistributionRatio)
                                {
                                    account.DistributionRatio = accountData.DistributionRatio;
                                }

                                if (accountData.Name != null && account.DisplayName != accountData.Name)
                                {
                                    account.DisplayName = accountData.Name;
                                }

                                if (account.Difficulty != accountData.Difficulty)
                                {
                                    account.Difficulty = accountData.Difficulty;
                                }

                                if (decimal.TryParse(accountData.Collateral, out var collateral))
                                {
                                    account.Collateral = collateral;
                                }

                                if (decimal.TryParse(accountData.Pending, out var pending))
                                {
                                    account.PendingBalance = pending;
                                }

                                try
                                {
                                    var historicalData = await postApiClient.GetAccountHistoricalAsync(account.LauncherId);
                                    if (historicalData != null)
                                    {
                                        foreach (var item in historicalData)
                                        {
                                            try
                                            {
                                                if (!_appDbContext.PostAccountHistoricalDbItems.Any(x => x.AccountId == account.Id && x.CreatedAt == Convert.ToUInt64(item.CreatedAt.ToUnixTimeMilliseconds())))
                                                {
                                                    account.PostAccountHistoricalDbItems.Add(PostAccountHistoricalDbItem.FromApiItem(item));
                                                }
                                                else
                                                {
                                                    // make sure nothing has changed for the given point
                                                    var record = _appDbContext.PostAccountHistoricalDbItems.First(x => x.AccountId == account.Id && x.CreatedAt == Convert.ToUInt64(item.CreatedAt.ToUnixTimeMilliseconds()));
                                                    if (record.ShareCount != item.ShareCount)
                                                    {
                                                        record.ShareCount = item.ShareCount;
                                                    }

                                                    if (record.Difficulty != item.Difficulty)
                                                    {
                                                        record.Difficulty = item.Difficulty;
                                                    }

                                                    if (record.EstCapacity != item.Ec)
                                                    {
                                                        record.EstCapacity = item.Ec;
                                                    }

                                                    if (record.Shares != item.Shares)
                                                    {
                                                        record.Shares = item.Shares;
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                _logger.LogError(ex, "Exception thrown while updating account historical item account: {Id}, created: {Created}, ec: {Ec}.", account.Id, item.CreatedAt, item.Ec);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Account updater failed to retrieve account historical data from api for account {Id}.", account.Id);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Exception thrown while updating historicals for account {Id}.", account.Id);
                                }

                                account.LastUpdated = Convert.ToUInt64(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

                                try
                                {
                                    await App.Current.Dispatcher.InvokeAsync(() => _appDbContext.SaveChanges());
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Exception thrown while updating account {Id} to db.", account.Id);
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Account updater failed to retrieve account data from api for account {Id}.", account.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Exception thrown while updating account {Id}.", account.Id);
                        }
                    }
                    else
                    {
                        // POC NOT YET SUPPORTED
                    }
                }

                try
                {
                    await App.Current.Dispatcher.InvokeAsync(() => _appDbContext.SaveChangesAsync());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception thrown while saving to db.");
                }

                var count = Interlocked.Increment(ref _executionCount);

                _logger.LogInformation("Account updater service completed a round. Count: {Count}", count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown in outer account updater service.");
            }
            finally
            {
                _ = _semaphore.Release();
            }
        }
    }
}
