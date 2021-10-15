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
    internal class PoolInfoUpdaterService : IHostedService, IDisposable
    {
        private readonly ILogger<PoolInfoUpdaterService> _logger;
        private readonly AppViewModel _appViewModel;
        private Timer _timer;
        private int _executionCount;
        private bool _disposedValue;

        public PoolInfoUpdaterService(AppViewModel appViewModel, ILogger<PoolInfoUpdaterService> logger)
        {
            _logger = logger;
            _appViewModel = appViewModel;
            Properties.Settings.Default.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Properties.Settings.Default.PoolInfoUpdateInterval))
            {
                if (_timer == null) return;

                _timer.Change(Properties.Settings.Default.PoolInfoUpdateInterval, Properties.Settings.Default.PoolInfoUpdateInterval);

                _logger.LogInformation("Pool info updater interval changed to {NewInterval}", Properties.Settings.Default.PoolInfoUpdateInterval);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pool info updater service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, Properties.Settings.Default.PoolInfoUpdateInterval);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pool info updater service is stopping.");

            _ = (_timer?.Change(Timeout.Infinite, 0));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            // do our work on the same thread as the dbcontext
            await _appViewModel.AppViewDispatcher.InvokeAsync(new Action(async () =>
            {
                foreach (var poolName in Enum.GetNames(typeof(PostPool)))
                {
                    using var postPoolApi = new PostApiClient((PostPool)Enum.Parse(typeof(PostPool), poolName, true));

                    var poolConfig = await postPoolApi.GetConfigAsync();
                    var pool = await postPoolApi.GetPoolAsync();
                    var rewards = await postPoolApi.GetRewardsAsync();
                    var payouts = await postPoolApi.GetPayoutsAsync();
                    var lastPayoutTime = payouts.Max(x => x.CreatedAt);

                    if (_appViewModel.FmDbContext.PostPools.Any(x => x.PoolUrl.ToLower() == poolConfig.PoolUrl.ToLower()))
                    {
                        // update the existing record
                        var record = _appViewModel.FmDbContext.PostPools.First(x => x.PoolUrl.ToLower() == poolConfig.PoolUrl.ToLower());
                        if (record.LastPayoutTime != lastPayoutTime)
                        {
                            // TODO: alert of payout
                        }

                        record.UpdateFromApiData(poolConfig, pool, rewards, lastPayoutTime);
                    }
                    else
                    {
                        // add new record
                        var newRecord = PostPoolInfo.FromApiData(poolConfig, pool, rewards, lastPayoutTime);
                        newRecord.PoolApiName = (PostPool)Enum.Parse(typeof(PostPool), poolName, true);
                        _ = _appViewModel.FmDbContext.PostPools.Add(newRecord);
                    }
                }
                _ = await _appViewModel.FmDbContext.SaveChangesAsync();
            }));

            var count = Interlocked.Increment(ref _executionCount);

            _logger.LogInformation("Pool info updater service completed a round. Count: {Count}", count);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _timer?.Dispose();
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
