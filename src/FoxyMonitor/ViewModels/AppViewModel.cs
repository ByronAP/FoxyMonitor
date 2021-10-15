using FoxyMonitor.Data;
using FoxyMonitor.Data.Models;
using FoxyMonitor.Utils;
using FoxyPoolApi;
using Microsoft.Extensions.Logging;
using SplotControl.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using static MoreLinq.Extensions.DistinctByExtension;

namespace FoxyMonitor.ViewModels
{
    public class AppViewModel : NotifyChangeBase, IDisposable
    {
        public Account SelectedAccount
        {
            get
            {
                try
                {
                    return FmDbContext.Accounts.First(x => x.Id.Equals(SelectedAccountId));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to get selected account.");
                    return null;
                }
            }
        }

        public PostPoolInfo SelectedPostPoolInfo { get => _selectedPostPoolInfo; }

        public uint SelectedAccountId
        {
            get => _selectedAccountId;
            set
            {
                OnPropertyChanging(nameof(SelectedAccount));
                OnPropertyChanging(nameof(SelectedAccountSharesSeries));
                OnPropertyChanging(nameof(SelectedAccountEstCapacitySeries));
                _ = SetField(ref _selectedAccountId, value);
                _ = SetField(ref _selectedPostPoolInfo, PostPools.FirstOrDefault(x => x.PoolApiName.Equals(Enum.Parse(typeof(PostPool), SelectedAccount.PoolName, true))), nameof(SelectedPostPoolInfo));
                OnPropertyChanged(nameof(SelectedAccount));
                OnPropertyChanged(nameof(SelectedAccountSharesSeries));
                OnPropertyChanged(nameof(SelectedAccountEstCapacitySeries));
            }
        }
        private uint _selectedAccountId;
        private PostPoolInfo _selectedPostPoolInfo;

        public string AppVersion => Assembly.GetEntryAssembly().GetName().Version.ToString();

        public int AccountsCount { get => FmDbContext.Accounts.Count(); }

        public int AlertsCount { get => FmDbContext.Alerts.Count(); }

        public ObservableCollection<Account> Accounts { get => FmDbContext.Accounts.Local.ToObservableCollection(); }

        public ObservableCollection<Alert> Alerts { get => FmDbContext.Alerts.Local.ToObservableCollection(); }

        public ObservableCollection<PostPoolInfo> PostPools { get => FmDbContext.PostPools.Local.ToObservableCollection(); }

        public Brush ChartBrush { get => _chartBrush; set => SetField(ref _chartBrush, value); }
        private Brush _chartBrush = SystemColors.ControlTextBrush;

        public GridOptions GridOptions { get => _gridOptions; set => SetField(ref _gridOptions, value); }
        private GridOptions _gridOptions = new GridOptions();

        public ColumnSeries SelectedAccountSharesSeries
        {
            get
            {
                if (SelectedAccount == null) return null;

                var colSeriesValues = new ObservableCollection<DateTimeDataPoint>();

                var startDT = DateTimeOffset.UtcNow.AddDays(-1);
                foreach (var point in SelectedAccount.PostAccountHistoricalDbItems.Where(x => x.CreatedAt.CompareTo(startDT) > 0).DistinctBy(x => x.CreatedAt).OrderBy(x => x.CreatedAt))
                {
                    colSeriesValues.Add(new DateTimeDataPoint { DTOffset = point.CreatedAt, Value = point.Shares });
                }

                return new ColumnSeries
                {
                    Name = "Shares",
                    Points = colSeriesValues,
                    PointFillBrush = ChartBrush,
                    PointStrokeBrush = ChartBrush,
                };
            }
        }

        public ColumnSeries SelectedAccountEstCapacitySeries
        {
            get
            {
                if (SelectedAccount == null) return null;

                var seriesValues = new ObservableCollection<DateTimeDataPoint>();

                var startDT = DateTimeOffset.UtcNow.AddDays(-1);
                foreach (var point in SelectedAccount.PostAccountHistoricalDbItems.Where(x => x.CreatedAt.CompareTo(startDT.DateTime) > 0).ToArray().DistinctBy(x => x.CreatedAt).OrderBy(x => x.CreatedAt))
                {
                    seriesValues.Add(new DateTimeDataPoint { DTOffset = point.CreatedAt, Value = (uint)point.EstCapacity });
                }

                return new ColumnSeries
                {
                    Name = "Est Capacity",
                    Points = seriesValues,
                    PointFillBrush = ChartBrush,
                    PointStrokeBrush = ChartBrush,
                };
            }
        }

        // we allow access to the dbcontext and the dispatcher so that the data can be updated elsewhere
        // and still retain property change notifications
        internal readonly Dispatcher AppViewDispatcher;
        internal readonly FMDbContext FmDbContext;
        private readonly ILogger<AppViewModel> _logger;
        private readonly Timer _15SecondTimer;
        private bool _disposedValue;

        public AppViewModel(ILogger<AppViewModel> logger)
        {
            AppViewDispatcher = Dispatcher.CurrentDispatcher;
            _logger = logger;
            FmDbContext = new FMDbContext();

            if (FmDbContext.Accounts.Any()) SelectedAccountId = FmDbContext.Accounts.First().Id;

            ReloadPostPoolsFromDb();

            ReloadAccountsFromDb();

            ReloadHistoricalFromDb();

            ReloadAlertsFromDb();

            _15SecondTimer = new Timer(On15SecondTimer_Tick, null, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15));
            _logger.LogDebug("New AppViewModel instance created.");
        }

        private void On15SecondTimer_Tick(object state)
        {
            if (SelectedAccount != null)
            {
                AppViewDispatcher.Invoke(() =>
                {
                    OnPropertyChanging(nameof(SelectedPostPoolInfo));
                    OnPropertyChanged(nameof(SelectedPostPoolInfo));
                    OnPropertyChanging(nameof(SelectedAccount));
                    OnPropertyChanged(nameof(SelectedAccount));
                    OnPropertyChanging(nameof(SelectedAccountSharesSeries));
                    OnPropertyChanged(nameof(SelectedAccountSharesSeries));
                    OnPropertyChanging(nameof(SelectedAccountEstCapacitySeries));
                    OnPropertyChanged(nameof(SelectedAccountEstCapacitySeries));
                });
            }
        }

        public void ReloadPostPoolsFromDb()
        {
            var pools = FmDbContext.PostPools.Where(x => x.Id >= 0).ToArray();
            Array.Clear(pools, 0, pools.Length);
        }

        public void ReloadAccountsFromDb()
        {
            var accounts = FmDbContext.Accounts.Where(x => x.Id >= 0).ToArray();
            Array.Clear(accounts, 0, accounts.Length);
        }

        public void ReloadAlertsFromDb()
        {
            var alerts = FmDbContext.Alerts.Where(x => x.Id >= 0).ToArray();
            Array.Clear(alerts, 0, alerts.Length);
        }

        public void ReloadHistoricalFromDb()
        {
            var startDT = DateTimeOffset.UtcNow.AddDays(-1);
            var historical = FmDbContext.PostAccountHistoricalDbItems.Where(x => x.CreatedAt.CompareTo(startDT) > 0).ToArray();
            Array.Clear(historical, 0, historical.Length);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _15SecondTimer.Dispose();
                    FmDbContext.Dispose();
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
