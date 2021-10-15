using FoxyMonitor.Data.Models;
using FoxyMonitor.ViewModels;
using FoxyPoolApi;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FoxyMonitor.Services
{
    internal class AccountUpdaterService : IHostedService, IDisposable
    {
        private readonly ILogger<AccountUpdaterService> _logger;
        private readonly AppViewModel _appViewModel;
        private readonly SemaphoreSlim _semaphore;
        private Timer _timer;
        private int _executionCount;
        private bool _disposedValue;

        public AccountUpdaterService(AppViewModel appViewModel, ILogger<AccountUpdaterService> logger)
        {
            _logger = logger;
            _appViewModel = appViewModel;
            _semaphore = new SemaphoreSlim(1);
            Properties.Settings.Default.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Properties.Settings.Default.AccountsUpdateInterval))
            {
                if (_timer == null) return;

                _timer.Change(Properties.Settings.Default.AccountsUpdateInterval, Properties.Settings.Default.AccountsUpdateInterval);

                _logger.LogInformation("Account updater interval changed to {NewInterval}", Properties.Settings.Default.AccountsUpdateInterval);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Account updater service running.");

            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(6), Properties.Settings.Default.AccountsUpdateInterval);

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
                await _semaphore.WaitAsync();

                // do our work on the same thread as the dbcontext
                await _appViewModel.AppViewDispatcher.InvokeAsync(new Action(async () =>
                {
                    foreach (var account in _appViewModel.Accounts)
                    {
                        if (account.PoolType == PoolType.POST)
                        {
                            try
                            {
                                using var postApiClient = new PostApiClient((PostPool)Enum.Parse(typeof(PostPool), account.PoolName, true));
                                var accountData = await postApiClient.GetAccountAsync(account.LauncherId);
                                if (accountData != null)
                                {
                                    if (account.PoolPubKey != accountData.PoolPublicKey)
                                    {
                                        account.PoolPubKey = accountData.PoolPublicKey;
                                        // TODO: Issue an alert
                                    }

                                    if (account.PayoutAddress != accountData.PayoutAddress)
                                    {
                                        account.PayoutAddress = accountData.PayoutAddress;
                                        // TODO: Issue an alert
                                    }

                                    if (account.Shares != accountData.Shares)
                                    {
                                        account.Shares = accountData.Shares;
                                    }

                                    if (account.LastAcceptedPartialTime != accountData.LastAcceptedPartialAt)
                                    {
                                        account.LastAcceptedPartialTime = accountData.LastAcceptedPartialAt;
                                    }

                                    if (account.EstCapacity != accountData.Ec)
                                    {
                                        account.EstCapacity = accountData.Ec;
                                    }

                                    if (account.DistributionRatio != accountData.DistributionRatio)
                                    {
                                        account.DistributionRatio = accountData.DistributionRatio;
                                    }

                                    if (account.DisplayName != accountData.Name)
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
                                                    if (!account.PostAccountHistoricalDbItems.Any(x => x.CreatedAt.Equals(item.CreatedAt)))
                                                    {
                                                        account.PostAccountHistoricalDbItems.Add(PostAccountHistoricalDbItem.FromApiItem(item));
                                                    }
                                                    else
                                                    {
                                                        // make sure nothing has changed for the given point
                                                        var record = account.PostAccountHistoricalDbItems.First(x => x.CreatedAt.Equals(item.CreatedAt));
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

                                    account.LastUpdated = DateTimeOffset.UtcNow;

                                    try
                                    {
                                        _ = await _appViewModel.FmDbContext.SaveChangesAsync();
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
                        _ = await _appViewModel.FmDbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception thrown while saving to db.");
                    }
                }));
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

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        _timer?.Dispose();
                    }
                    catch
                    {
                        // ignore, could already be disposed
                    }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
