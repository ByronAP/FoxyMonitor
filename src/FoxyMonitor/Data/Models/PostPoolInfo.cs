using FoxyMonitor.Utils;
using FoxyPoolApi;
using FoxyPoolApi.Responses;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoxyMonitor.Data.Models
{
    [Table("postPools")]
    public class PostPoolInfo : NotifyChangeBase
    {
        [Key]
        [Column("id")]
        public uint Id { get; set; }

        [Column("poolApiName")]
        [Required]
        public PostPool PoolApiName { get => _poolApiName; set => SetField(ref _poolApiName, value); }
        private PostPool _poolApiName;

        [Column("poolUrl")]
        public string PoolUrl { get => _poolUrl; set => SetField(ref _poolUrl, value); }
        private string _poolUrl;

        [Column("blockExplorerBlockUrlTemplate")]
        public string BlockExplorerBlockUrlTemplate { get => _blockExplorerBlockUrlTemplate; set => SetField(ref _blockExplorerBlockUrlTemplate, value); }
        private string _blockExplorerBlockUrlTemplate;

        [Column("blockExplorerCoinUrlTemplate")]
        public string BlockExplorerCoinUrlTemplate { get => _blockExplorerCoinUrlTemplate; set => SetField(ref _blockExplorerCoinUrlTemplate, value); }
        private string _blockExplorerCoinUrlTemplate;

        [Column("blockExplorerAddressUrlTemplate")]
        public string BlockExplorerAddressUrlTemplate { get => _blockExplorerAddressUrlTemplate; set => SetField(ref _blockExplorerAddressUrlTemplate, value); }
        private string _blockExplorerAddressUrlTemplate;

        [Column("blockRewardDistributionDelay")]
        public uint BlockRewardDistributionDelay { get => _blockRewardDistributionDelay; set => SetField(ref _blockRewardDistributionDelay, value); }
        private uint _blockRewardDistributionDelay;

        [Column("blocksPerDay")]
        public uint BlocksPerDay { get => _blocksPerDay; set => SetField(ref _blocksPerDay, value); }
        private uint _blocksPerDay;

        [Column("defaultDistributionRatio")]
        public string DefaultDistributionRatio { get => _defaultDistributionRatio; set => SetField(ref _defaultDistributionRatio, value); }
        private string _defaultDistributionRatio;

        [Column("historicalTimeInMinutes")]
        public uint HistoricalTimeInMinutes { get => _historicalTimeMinutes; set => SetField(ref _historicalTimeMinutes, value); }
        private uint _historicalTimeMinutes;

        [Column("minimumPayout")]
        public decimal MinimumPayout { get => _minimumPayout; set => SetField(ref _minimumPayout, value); }
        private decimal _minimumPayout;

        [Column("onDemandPayoutFee")]
        public decimal OnDemandPayoutFee { get => _onDemandPayoutFee; set => SetField(ref _onDemandPayoutFee, value); }
        private decimal _onDemandPayoutFee;

        [Column("poolFee")]
        public decimal PoolFee { get => _poolFee; set => SetField(ref _poolFee, value); }
        private decimal _poolFee;

        [Column("coin")]
        public string Coin { get => _coin; set => SetField(ref _coin, value); }
        private string _coin;

        [Column("ticker")]
        public string Ticker { get => _ticker; set => SetField(ref _ticker, value); }
        private string _ticker;

        [Column("version")]
        public string Version { get => _version; set => SetField(ref _version, value); }
        private string _version;

        [Column("isTestnet")]
        public bool IsTestnet { get => _isTestnet; set => SetField(ref _isTestnet, value); }
        private bool _isTestnet;

        [Column("poolAddress")]
        public string PoolAddress { get => _poolAddress; set => SetField(ref _poolAddress, value); }
        private string _poolAddress;

        [Column("poolName")]
        public string PoolName { get => _poolName; set => SetField(ref _poolName, value); }
        private string _poolName;

        [Column("farmingUrl")]
        public string FarmingUrl { get => _farmingUrl; set => SetField(ref _farmingUrl, value); }
        private string _farmingUrl;

        [Column("height")]
        public ulong Height { get => _height; set => SetField(ref _height, value); }
        private ulong _height;

        [Column("difficulty")]
        public uint Difficulty { get => _difficulty; set => SetField(ref _difficulty, value); }
        private uint _difficulty;

        [Column("receivedAt")]
        public DateTimeOffset ReceivedAt { get => _receivedAt; set => SetField(ref _receivedAt, value); }
        private DateTimeOffset _receivedAt;

        [Column("networkSpaceInTiB")]
        public string NetworkSpaceInTiB { get => _networkSpaceInTiB; set => SetField(ref _networkSpaceInTiB, value); }
        private string _networkSpaceInTiB;

        [Column("balance")]
        public string Balance { get => _balance; set => SetField(ref _balance, value); }
        private string _balance;

        [Column("avgEffort")]
        public decimal AverageEffort { get => _averageEffort; set => SetField(ref _averageEffort, value); }
        private decimal _averageEffort;

        [Column("dailyRewardPerPiB")]
        public decimal DailyRewardPerPiB { get => _dailyRewardPerPiB; set => SetField(ref _dailyRewardPerPiB, value); }
        private decimal _dailyRewardPerPiB;

        //TODO: add historical once it is added to the api client

        [Column("lastPayoutTime")]
        public DateTimeOffset LastPayoutTime { get => _lastPayoutTime; set => SetField(ref _lastPayoutTime, value); }
        private DateTimeOffset _lastPayoutTime;

        [Column("lastUpdated")]
        public DateTimeOffset LastUpdated { get => _lastUpdated; set => SetField(ref _lastUpdated, value); }
        private DateTimeOffset _lastUpdated = DateTimeOffset.UtcNow;

        public static PostPoolInfo FromApiData(PostConfigResponse config, PostPoolResponse pool, PostRewardsResponse rewards, DateTimeOffset lastPayoutTime)
        {
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
                ReceivedAt = pool.ReceivedAt,
                NetworkSpaceInTiB = pool.NetworkSpaceInTiB,
                Balance = pool.Balance,
                AverageEffort = rewards.AverageEffort,
                DailyRewardPerPiB = rewards.DailyRewardPerPiB,
                LastPayoutTime = lastPayoutTime
            };
        }

        public void UpdateFromApiData(PostConfigResponse config, PostPoolResponse pool, PostRewardsResponse rewards, DateTimeOffset lastPayoutTime)
        {
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
            ReceivedAt = pool.ReceivedAt;
            NetworkSpaceInTiB = pool.NetworkSpaceInTiB;
            Balance = pool.Balance;
            AverageEffort = rewards.AverageEffort;
            DailyRewardPerPiB = rewards.DailyRewardPerPiB;
            LastPayoutTime = lastPayoutTime;
        }
    }
}
