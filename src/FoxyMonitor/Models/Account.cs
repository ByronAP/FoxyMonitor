using FoxyPoolApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace FoxyMonitor.Models
{
    [Index(nameof(LauncherId), nameof(PoolName), IsUnique = true)]
    public class Account : ObservableObject
    {
        [Key]
        public uint Id { get => _id; set => SetProperty(ref _id, value); }
        private uint _id;

        public string DisplayName { get => _displayName; set => SetProperty(ref _displayName, value); }
        private string _displayName = string.Empty;

        public PoolType PoolType { get => _poolType; set => SetProperty(ref _poolType, value); }
        private PoolType _poolType = PoolType.POST;

        public string PoolName { get => _poolName; set => SetProperty(ref _poolName, value); }
        private string _poolName = PostPool.Chia_OG.ToString();

        /// <summary>
        /// Launcher id is also known as the Pool Public Key
        /// </summary>
        public string LauncherId { get => _launcerId; set => SetProperty(ref _launcerId, value); }
        private string _launcerId = string.Empty;

        /// <summary>
        /// This is the SSO auth token from the web login
        /// </summary>
        public string AuthToken { get => _authToken; set => SetProperty(ref _authToken, value); }
        private string _authToken = string.Empty;

        /// <summary>
        /// The last block height won
        /// </summary>
        public ulong LastBlockWon { get => _lastBlockWon; set => SetProperty(ref _lastBlockWon, value); }
        private ulong _lastBlockWon;

        /// <summary>
        /// The last time the pool accepted a partial (USE UTC ONLY, ms)
        /// </summary>
        public ulong LastAcceptedPartialTime { get => _lastAcceptedPartialTime; set => SetProperty(ref _lastAcceptedPartialTime, value); }
        private ulong _lastAcceptedPartialTime;

        /// <summary>
        /// Pending pool payout balance
        /// </summary>
        public decimal PendingBalance { get => _pendingBalance; set => SetProperty(ref _pendingBalance, value); }
        private decimal _pendingBalance;

        /// <summary>
        /// Current shares
        /// </summary>
        public uint Shares { get => _shares; set => SetProperty(ref _shares, value); }
        private uint _shares;

        /// <summary>
        /// Estimated capacity
        /// </summary>
        public decimal EstCapacity { get => _estCapacity; set => SetProperty(ref _estCapacity, value); }
        private decimal _estCapacity;

        /// <summary>
        /// Amount being held as collateral
        /// </summary>
        public decimal Collateral { get => _collateral; set => SetProperty(ref _collateral, value); }
        private decimal _collateral;

        /// <summary>
        /// Current partials difficulty
        /// </summary>
        public uint Difficulty { get => _difficulty; set => SetProperty(ref _difficulty, value); }
        private uint _difficulty;

        /// <summary>
        /// Distribution ratio
        /// </summary>
        public string DistributionRatio { get => _distributionRatio; set => SetProperty(ref _distributionRatio, value); }
        private string _distributionRatio = string.Empty;

        /// <summary>
        /// Address that payouts are sent to
        /// </summary>
        public string PayoutAddress { get => _payoutAddress; set => SetProperty(ref _payoutAddress, value); }
        private string _payoutAddress = string.Empty;

        /// <summary>
        /// The pools public key
        /// </summary>
        public string PoolPubKey { get => _poolPubKey; set => SetProperty(ref _poolPubKey, value); }
        private string _poolPubKey = string.Empty;

        /// <summary>
        /// Est Capacity and Shares history
        /// </summary>
        public ObservableCollection<PostAccountHistoricalDbItem> PostAccountHistoricalDbItems { get => _postAccountHistoricalDbItems; set => SetProperty(ref _postAccountHistoricalDbItems, value); }
        private ObservableCollection<PostAccountHistoricalDbItem> _postAccountHistoricalDbItems = new ObservableCollection<PostAccountHistoricalDbItem>();

        /// <summary>
        /// Balance history
        /// </summary>
        public ObservableCollection<AccountBalanceHistoricalDbItem> AccountBalanceHistoricalDbItems { get => _accountBalanceHistoricalDbItems; set => SetProperty(ref _accountBalanceHistoricalDbItems, value); }
        private ObservableCollection<AccountBalanceHistoricalDbItem> _accountBalanceHistoricalDbItems = new ObservableCollection<AccountBalanceHistoricalDbItem>();

        /// <summary>
        /// The last time this accounts data was updated from the pool server (USE UTC ONLY, ms)
        /// </summary>
        public ulong LastUpdated { get => _lastUpdated; set => SetProperty(ref _lastUpdated, value); }
        private ulong _lastUpdated;

        public bool AlertOnSlowPartials { get => _alertOnSlowPartials; set => SetProperty(ref _alertOnSlowPartials, value); }
        private bool _alertOnSlowPartials = true;

        public bool AlertOnBlockWon { get => _alertOnBlockWon; set => SetProperty(ref _alertOnBlockWon, value); }
        private bool _alertOnBlockWon = true;

        public decimal PayoutAddressBalance { get => _payoutAddressBalance; set => SetProperty(ref _payoutAddressBalance, value); }
        private decimal _payoutAddressBalance = 0m;

        public TimeSpan MaxPartialTime { get => _maxPartialTime; set => SetProperty(ref _maxPartialTime, value); }
        private TimeSpan _maxPartialTime = TimeSpan.FromMinutes(20);
    }
}
