using FoxyPoolApi.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FoxyMonitor.Models
{
    [Index(nameof(AccountId), nameof(CreatedAt), IsUnique = true)]
    public class PostAccountHistoricalDbItem : ObservableObject
    {
        [Key]
        public uint Id { get => _id; set => SetProperty(ref _id, value); }
        private uint _id;

        public uint AccountId { get => _accountId; set => SetProperty(ref _accountId, value); }
        private uint _accountId;

        public decimal EstCapacity { get => _estCapacity; set => SetProperty(ref _estCapacity, value); }
        private decimal _estCapacity;

        public uint ShareCount { get => _shareCount; set => SetProperty(ref _shareCount, value); }
        private uint _shareCount;

        /// <summary>
        /// Gets or sets the created at (USE UTC ONLY, ms)
        /// </summary>
        /// <value>The created at.</value>
        public ulong CreatedAt { get => _createdAt; set => SetProperty(ref _createdAt, value); }
        private ulong _createdAt;

        public uint Difficulty { get => _difficulty; set => SetProperty(ref _difficulty, value); }
        private uint _difficulty;

        public uint Shares { get => _shares; set => SetProperty(ref _shares, value); }
        private uint _shares;

        public static PostAccountHistoricalDbItem FromApiItem(PostAccountHistoricalItem value)
        {
            return new PostAccountHistoricalDbItem
            {
                EstCapacity = value.Ec,
                ShareCount = value.ShareCount,
                CreatedAt = (ulong)value.CreatedAt.ToUnixTimeMilliseconds(),
                Difficulty = value.Difficulty,
                Shares = value.Shares
            };
        }
    }
}
