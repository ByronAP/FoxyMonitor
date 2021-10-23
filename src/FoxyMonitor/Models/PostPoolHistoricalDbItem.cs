using FoxyPoolApi.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FoxyMonitor.Models
{
    [Index(nameof(PostPoolId), nameof(Timestamp), IsUnique = true)]
    public class PostPoolHistoricalDbItem : ObservableObject
    {
        [Key]
        public uint Id { get => _id; set => SetProperty(ref _id, value); }
        private uint _id;

        public uint PostPoolId { get => _postPoolId; set => SetProperty(ref _postPoolId, value); }
        private uint _postPoolId;

        /// <summary>
        /// Gets or sets the timestamp (UTC ONLY, ms).
        /// </summary>
        /// <value>The timestamp.</value>
        public ulong Timestamp { get => _timestamp; set => SetProperty(ref _timestamp, value); }
        private ulong _timestamp;

        public ulong Blocks { get => _blocks; set => SetProperty(ref _blocks, value); }
        private ulong _blocks;

        public decimal Effort { get => _effort; set => SetProperty(ref _effort, value); }
        private decimal _effort;

        public string PoolEcInTib { get => _poolEcInTib; set => SetProperty(ref _poolEcInTib, value); }
        private string _poolEcInTib = string.Empty;

        public string NetworkCapacityInTib { get => _networkCapacityInTib; set => SetProperty(ref _networkCapacityInTib, value); }
        private string _networkCapacityInTib = string.Empty;

        public static PostPoolHistoricalDbItem FromApiItem(uint postPoolId, PostPoolHistoricalItem value)
        {
            return new PostPoolHistoricalDbItem
            {
                PostPoolId = postPoolId,
                Timestamp = (ulong)value.Timestamp.ToUnixTimeMilliseconds(),
                Blocks = (ulong)value.Blocks,
                Effort = value.Effort == null ? 0m : (decimal)value.Effort,
                PoolEcInTib = value.PoolEcInTib,
                NetworkCapacityInTib = value.NetworkCapacityInTib
            };
        }
    }
}
