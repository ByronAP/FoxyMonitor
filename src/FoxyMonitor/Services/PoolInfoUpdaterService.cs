using FoxyMonitor.Contracts.Services;
using FoxyMonitor.DbContexts;
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
    internal class PoolInfoUpdaterService : IHostedService
    {
        private const string PoolInfoUpdateIntervalPropertyKeyName = "PoolInfoUpdateInterval";
        private readonly ILogger<PoolInfoUpdaterService> _logger;
        private readonly AppDbContext _appDbContext;
        private readonly ApplicationPropertiesService _appPropertiesService;
        private readonly SemaphoreSlim _semaphore;
        private Timer _timer;
        private int _executionCount;
        private TimeSpan _interval;

        public PoolInfoUpdaterService(ILogger<PoolInfoUpdaterService> logger, AppDbContext appDbContext, IApplicationPropertiesService appPropertiesService)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _appPropertiesService = (ApplicationPropertiesService)appPropertiesService;
            _semaphore = new SemaphoreSlim(1);

            if (appPropertiesService.Contains(PoolInfoUpdateIntervalPropertyKeyName))
            {
                _interval = TimeSpan.FromSeconds(appPropertiesService.GetProperty<uint>(PoolInfoUpdateIntervalPropertyKeyName));
            }
            else
            {
                _interval = TimeSpan.FromSeconds(15);
            }

            _appPropertiesService.PropertyChanged += AppPropertiesService_PropertyChanged;
        }

        private void AppPropertiesService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_timer == null || e.PropertyName != PoolInfoUpdateIntervalPropertyKeyName) return;

            var newInterval = TimeSpan.FromSeconds(_appPropertiesService.GetProperty<uint>(PoolInfoUpdateIntervalPropertyKeyName));

            if (_interval != newInterval)
            {
                _timer.Change(TimeSpan.FromSeconds(1), newInterval);

                _interval = newInterval;

                _logger.LogInformation("Pool info updater interval changed to {NewInterval}", newInterval);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pool info updater service running.");

            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(1), _interval);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pool info updater service is stopping.");

            _ = _timer.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                // we use this semaphore to make sure even if a run takes a long time we cant overlap
                await _semaphore.WaitAsync();

                foreach (var poolName in Enum.GetNames(typeof(PostPool)))
                {
                    using var postPoolApi = new PostApiClient((PostPool)Enum.Parse(typeof(PostPool), poolName, true));

                    var poolConfig = await postPoolApi.GetConfigAsync();
                    var pool = await postPoolApi.GetPoolAsync();
                    var rewards = await postPoolApi.GetRewardsAsync();
                    var payouts = await postPoolApi.GetPayoutsAsync();
                    var lastPayoutTime = payouts.Max(x => x.CreatedAt);

                    if (poolConfig != null && poolConfig.PoolUrl != null && _appDbContext.PostPools.Any(x => x.PoolUrl.ToLower() == poolConfig.PoolUrl.ToLower()))
                    {
                        // update the existing record
                        var record = _appDbContext.PostPools.First(x => x.PoolUrl.ToLower() == poolConfig.PoolUrl.ToLower());
                        if (record.LastPayoutTime != (ulong)lastPayoutTime.ToUnixTimeMilliseconds())
                        {
                            // TODO: alert of payout
                        }

                        await App.Current.Dispatcher.BeginInvoke(new Action(() =>
                          {
                              record.UpdateFromApiData(poolConfig, pool, rewards, lastPayoutTime);
                          }));
                    }
                    else if (poolConfig != null)
                    {
                        // add new record
                        var newRecord = PostPoolInfo.FromApiData(poolConfig, pool, rewards, lastPayoutTime);
                        newRecord.PoolApiName = (PostPool)Enum.Parse(typeof(PostPool), poolName, true);
                        _ = _appDbContext.PostPools.Add(newRecord);
                    }
                }

                await App.Current.Dispatcher.BeginInvoke(new Action(async () =>
                  {
                      try
                      {
                          _ = await _appDbContext.SaveChangesAsync();
                      }
                      catch (Exception ex)
                      {
                          _logger.LogError(ex, "Failed to save db data in pool updater service.");
                      }
                  }));

                var count = Interlocked.Increment(ref _executionCount);

                _logger.LogInformation("Pool info updater service completed a round. Count: {Count}", count);
            }
            catch (Exception ex)
            {
                try
                {
                    _logger.LogError(ex, "Pool info updater service threw an exception.");
                }
                catch
                {
                    // ignore
                }
            }
            finally
            {
                _ = _semaphore.Release();
            }
        }
    }
}
