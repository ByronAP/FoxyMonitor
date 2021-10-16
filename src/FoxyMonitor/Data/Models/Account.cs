using FoxyMonitor.Utils;
using FoxyPoolApi;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoxyMonitor.Data.Models
{
    // PK is built using DisplayName and the PoolType
    [Table("accounts")]
    public class Account : NotifyChangeBase
    {
        [Key]
        [Column("id")]
        public uint Id { get; set; }

        [Required]
        [Column("displayName")]
        public string DisplayName { get => _displayName; set => SetField(ref _displayName, value); }
        private string _displayName = string.Empty;

        [Required]
        [Column("poolType")]
        public PoolType PoolType { get => _poolType; set => SetField(ref _poolType, value); }
        private PoolType _poolType = PoolType.POST;

        [Column("poolName")]
        public string PoolName { get => _poolName; set => SetField(ref _poolName, value); }
        private string _poolName = PostPool.Chia_OG.ToString();

        /// <summary>
        /// Launcher id is also known as the Pool Public Key
        /// </summary>
        [Required]
        [Column("launcherId")]
        public string LauncherId { get => _launcerId; set => SetField(ref _launcerId, value); }
        private string _launcerId = string.Empty;

        /// <summary>
        /// This is the SSO auth token from the web login
        /// </summary>
        [Column("authToken")]
        public string AuthToken { get => _authToken; set => SetField(ref _authToken, value); }
        private string _authToken = string.Empty;

        /// <summary>
        /// The last block height won
        /// </summary>
        [Column("lastBlockWon")]
        public ulong LastBlockWon { get => _lastBlockWon; set => SetField(ref _lastBlockWon, value); }
        private ulong _lastBlockWon;

        /// <summary>
        /// The last time the pool accepted a partial
        /// </summary>
        [Column("lastAcceptedPartialTime")]
        public DateTimeOffset LastAcceptedPartialTime { get => _lastAcceptedPartialTime; set => SetField(ref _lastAcceptedPartialTime, value); }
        private DateTimeOffset _lastAcceptedPartialTime;

        /// <summary>
        /// Pending pool payout balance
        /// </summary>
        [Column("pendingBalance")]
        public decimal PendingBalance { get => _pendingBalance; set => SetField(ref _pendingBalance, value); }
        private decimal _pendingBalance;

        /// <summary>
        /// Current shares
        /// </summary>
        [Column("shares")]
        public uint Shares { get => _shares; set => SetField(ref _shares, value); }
        private uint _shares;

        /// <summary>
        /// Estimated capacity
        /// </summary>
        [Column("estCapacity")]
        public decimal EstCapacity { get => _estCapacity; set => SetField(ref _estCapacity, value); }
        private decimal _estCapacity;

        /// <summary>
        /// Amount being held as collateral
        /// </summary>
        [Column("collateral")]
        public decimal Collateral { get => _collateral; set => SetField(ref _collateral, value); }
        private decimal _collateral;

        /// <summary>
        /// Current partials difficulty
        /// </summary>
        [Column("difficulty")]
        public uint Difficulty { get => _difficulty; set => SetField(ref _difficulty, value); }
        private uint _difficulty;

        /// <summary>
        /// Distribution ratio
        /// </summary>
        [Column("distRatio")]
        public string DistributionRatio { get => _distributionRatio; set => SetField(ref _distributionRatio, value); }
        private string _distributionRatio = string.Empty;

        /// <summary>
        /// Address that payouts are sent to
        /// </summary>
        [Column("payoutAddress")]
        public string PayoutAddress { get => _payoutAddress; set => SetField(ref _payoutAddress, value); }
        private string _payoutAddress = string.Empty;

        /// <summary>
        /// The pools public key
        /// </summary>
        [Column("poolPubKey")]
        public string PoolPubKey { get => _poolPubKey; set => SetField(ref _poolPubKey, value); }
        private string _poolPubKey = string.Empty;

        /// <summary>
        /// Est Capacity and Shares history
        /// </summary>
        [Column("historicalItems")]
        public ObservableCollection<PostAccountHistoricalDbItem> PostAccountHistoricalDbItems { get => _postAccountHistoricalDbItems; set => SetField(ref _postAccountHistoricalDbItems, value); }
        private ObservableCollection<PostAccountHistoricalDbItem> _postAccountHistoricalDbItems = new ObservableCollection<PostAccountHistoricalDbItem>();


        /// <summary>
        /// The last time this accounts data was updated from the pool server
        /// </summary>
        [Required]
        [Column("lastUpdated")]
        public DateTimeOffset LastUpdated { get => _lastUpdated; set => SetField(ref _lastUpdated, value); }
        private DateTimeOffset _lastUpdated;
    }
}
