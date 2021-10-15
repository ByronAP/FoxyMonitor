using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FoxyMonitor.Services
{
    internal class CheckForUpdatesService : IHostedService, IDisposable
    {
        private readonly TimeSpan UpdateCheckInterval;
        private readonly ILogger<CheckForUpdatesService> _logger;
        private Timer _timer;
        private bool _disposedValue;

        public CheckForUpdatesService(ILogger<CheckForUpdatesService> logger)
        {
            _logger = logger;
            UpdateCheckInterval = TimeSpan.FromHours(6);
            Properties.Settings.Default.PropertyChanged += Settings_PropertyChanged;
        }

        private async void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Properties.Settings.Default.AutoCheckForUpdates))
            {
                if (_timer == null) return;

                if (Properties.Settings.Default.AutoCheckForUpdates)
                {
                    await StartAsync(CancellationToken.None);
                }
                else
                {
                    await StopAsync(CancellationToken.None);
                }

                _logger.LogInformation("Check for updates service state changed to {EnabledText}.", Properties.Settings.Default.AutoCheckForUpdates ? "enabled" : "disabled");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!Properties.Settings.Default.AutoCheckForUpdates)
            {
                _timer = new Timer(DoWork, null, Timeout.Infinite, 0);
            }
            else
            {
                _logger.LogInformation("Check for updates service running.");

                _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(30), UpdateCheckInterval);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Check for updates service is stopping.");

            _ = (_timer?.Change(Timeout.Infinite, 0));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            // TODO: Check For Updates
            await Task.Delay(2000);
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
