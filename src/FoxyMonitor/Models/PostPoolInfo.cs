using FoxyPoolApi;
using FoxyPoolApi.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace FoxyMonitor.Models
{
    [Index(nameof(PoolApiName), IsUnique = true)]
    public class PostPoolInfo : ObservableObject
    {
        [Key]
        public uint Id { get => _id; set => SetProperty(ref _id, value); }
        private uint _id;

        public PostPool PoolApiName { get => _poolApiName; set => SetProperty(ref _poolApiName, value); }
        private PostPool _poolApiName;

        public string PoolUrl { get => _poolUrl; set => SetProperty(ref _poolUrl, value); }
        private string _poolUrl = string.Empty;

        public string BlockExplorerBlockUrlTemplate { get => _blockExplorerBlockUrlTemplate; set => SetProperty(ref _blockExplorerBlockUrlTemplate, value); }
        private string _blockExplorerBlockUrlTemplate = string.Empty;

        public string BlockExplorerCoinUrlTemplate { get => _blockExplorerCoinUrlTemplate; set => SetProperty(ref _blockExplorerCoinUrlTemplate, value); }
        private string _blockExplorerCoinUrlTemplate = string.Empty;

        public string BlockExplorerAddressUrlTemplate { get => _blockExplorerAddressUrlTemplate; set => SetProperty(ref _blockExplorerAddressUrlTemplate, value); }
        private string _blockExplorerAddressUrlTemplate = string.Empty;

        public uint BlockRewardDistributionDelay { get => _blockRewardDistributionDelay; set => SetProperty(ref _blockRewardDistributionDelay, value); }
        private uint _blockRewardDistributionDelay;

        public uint BlocksPerDay { get => _blocksPerDay; set => SetProperty(ref _blocksPerDay, value); }
        private uint _blocksPerDay;

        public string DefaultDistributionRatio { get => _defaultDistributionRatio; set => SetProperty(ref _defaultDistributionRatio, value); }
        private string _defaultDistributionRatio = string.Empty;

        public uint HistoricalTimeInMinutes { get => _historicalTimeMinutes; set => SetProperty(ref _historicalTimeMinutes, value); }
        private uint _historicalTimeMinutes;

        public decimal MinimumPayout { get => _minimumPayout; set => SetProperty(ref _minimumPayout, value); }
        private decimal _minimumPayout;

        public decimal OnDemandPayoutFee { get => _onDemandPayoutFee; set => SetProperty(ref _onDemandPayoutFee, value); }
        private decimal _onDemandPayoutFee;

        public decimal PoolFee { get => _poolFee; set => SetProperty(ref _poolFee, value); }
        private decimal _poolFee;

        public string Coin { get => _coin; set => SetProperty(ref _coin, value); }
        private string _coin = string.Empty;

        public string Ticker { get => _ticker; set => SetProperty(ref _ticker, value); }
        private string _ticker = string.Empty;

        public string Version { get => _version; set => SetProperty(ref _version, value); }
        private string _version = string.Empty;

        public bool IsTestnet { get => _isTestnet; set => SetProperty(ref _isTestnet, value); }
        private bool _isTestnet;

        public string PoolAddress { get => _poolAddress; set => SetProperty(ref _poolAddress, value); }
        private string _poolAddress = string.Empty;

        public string PoolName { get => _poolName; set => SetProperty(ref _poolName, value); }
        private string _poolName = string.Empty;

        public string FarmingUrl { get => _farmingUrl; set => SetProperty(ref _farmingUrl, value); }
        private string _farmingUrl = string.Empty;

        public ulong Height { get => _height; set => SetProperty(ref _height, value); }
        private ulong _height;

        public uint Difficulty { get => _difficulty; set => SetProperty(ref _difficulty, value); }
        private uint _difficulty;

        /// <summary>
        /// Gets or sets the received at (USE UTC ONLY, ms).
        /// </summary>
        /// <value>The received at.</value>
        public ulong ReceivedAt { get => _receivedAt; set => SetProperty(ref _receivedAt, value); }
        private ulong _receivedAt;

        public string NetworkSpaceInTiB { get => _networkSpaceInTiB; set => SetProperty(ref _networkSpaceInTiB, value); }
        private string _networkSpaceInTiB = string.Empty;

        public string Balance { get => _balance; set => SetProperty(ref _balance, value); }
        private string _balance = string.Empty;

        public decimal AverageEffort { get => _averageEffort; set => SetProperty(ref _averageEffort, value); }
        private decimal _averageEffort;

        public decimal DailyRewardPerPiB { get => _dailyRewardPerPiB; set => SetProperty(ref _dailyRewardPerPiB, value); }
        private decimal _dailyRewardPerPiB;

        /// <summary>
        /// Gets or sets the last payout time (USE UTC ONLY, ms).
        /// </summary>
        /// <value>The last payout time.</value>
        public ulong LastPayoutTime { get => _lastPayoutTime; set => SetProperty(ref _lastPayoutTime, value); }
        private ulong _lastPayoutTime;

        /// <summary>
        /// Gets or sets the last updated (USE UTC ONLY, ms).
        /// </summary>
        /// <value>The last updated.</value>
        public ulong LastUpdated { get => _lastUpdated; set => SetProperty(ref _lastUpdated, value); }
        private ulong _lastUpdated = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public ObservableCollection<PostPoolHistoricalDbItem> PoolHistoricalDbItems { get => _poolHistoricalDbItems; set => SetProperty(ref _poolHistoricalDbItems, value); }
        private ObservableCollection<PostPoolHistoricalDbItem> _poolHistoricalDbItems = new ObservableCollection<PostPoolHistoricalDbItem>();

        public static PostPoolInfo FromApiData(PostConfigResponse config, PostPoolResponse pool, PostRewardsResponse rewards, DateTimeOffset lastPayoutTime)
        {
            //TODO: add pool historical
            return new PostPoolInfo
            {
                PoolUrl = config.PoolUrl,
                BlockExplorerBlockUrlTemplate = config.BlockExplorerBlockUrlTemplate,
                BlockExplorerCoinUrlTemplate = config.BlockExplorerCoinUrlTemplate,
                BlockExplorerAddressUrlTemplate = config.BlockExplorerAddressUrlTemplate,
                BlockRewardDistributionDelay = config.BlockRewardDistributionDelay,
                BlocksPerDay = config.BlocksPerDay,
                DefaultDistributionRatio = config.DefaultDistributionRatio,
                HistoricalTimeInMinutes = config.HistoricalTimeInMinutes,
                MinimumPayout = config.MinimumPayout,
                OnDemandPayoutFee = config.OnDemandPayoutFee,
                PoolFee = config.PoolFee,
                Coin = config.Coin,
                Ticker = config.Ticker,
                Version = config.Version,
                IsTestnet = config.IsTestnet,
                PoolAddress = config.PoolAddress,
                PoolName = config.PoolName,
                FarmingUrl = config.FarmingUrl,
                Height = pool.Height,
                Difficulty = pool.Difficulty,
                ReceivedAt = (ulong)pool.ReceivedAt.ToUnixTimeMilliseconds(),
                NetworkSpaceInTiB = pool.NetworkSpaceInTiB,
                Balance = pool.Balance,
                AverageEffort = rewards.AverageEffort,
                DailyRewardPerPiB = rewards.DailyRewardPerPiB,
                LastPayoutTime = (ulong)lastPayoutTime.ToUnixTimeMilliseconds(),
            };
        }

        public void UpdateFromApiData(PostConfigResponse config, PostPoolResponse pool, PostRewardsResponse rewards, DateTimeOffset lastPayoutTime)
        {
            //TODO: add pool historical
            PoolUrl = config.PoolUrl;
            BlockExplorerBlockUrlTemplate = config.BlockExplorerBlockUrlTemplate;
            BlockExplorerCoinUrlTemplate = config.BlockExplorerCoinUrlTemplate;
            BlockExplorerAddressUrlTemplate = config.BlockExplorerAddressUrlTemplate;
            BlockRewardDistributionDelay = config.BlockRewardDistributionDelay;
            BlocksPerDay = config.BlocksPerDay;
            DefaultDistributionRatio = config.DefaultDistributionRatio;
            HistoricalTimeInMinutes = config.HistoricalTimeInMinutes;
            MinimumPayout = config.MinimumPayout;
            OnDemandPayoutFee = config.OnDemandPayoutFee;
            PoolFee = config.PoolFee;
            Coin = config.Coin;
            Ticker = config.Ticker;
            Version = config.Version;
            IsTestnet = config.IsTestnet;
            PoolAddress = config.PoolAddress;
            PoolName = config.PoolName;
            FarmingUrl = config.FarmingUrl;
            Height = pool.Height;
            Difficulty = pool.Difficulty;
            ReceivedAt = (ulong)pool.ReceivedAt.ToUnixTimeMilliseconds();
            NetworkSpaceInTiB = pool.NetworkSpaceInTiB;
            Balance = pool.Balance;
            AverageEffort = rewards.AverageEffort;
            DailyRewardPerPiB = rewards.DailyRewardPerPiB;
            LastPayoutTime = (ulong)lastPayoutTime.ToUnixTimeMilliseconds();
        }
    }
}
