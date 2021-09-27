using FoxyMonitor.Utils;
using FoxyPoolApi.Responses;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoxyMonitor.Data.Models
{
    [Table("postAccountHistoricalItems")]
    public class PostAccountHistoricalDbItem: NotifyChangeBase
    {
        [Key]
        [Column("id")]
        public uint Id { get => _id; set => SetField(ref _id, value); }
        private uint _id;
        
        [Required]
        [Column("accountId")]
        public uint AccountId { get; set; }

        [Column("estCapacity")]
        public decimal EstCapacity { get => _estCapacity; set => SetField(ref _estCapacity, value); }
        private decimal _estCapacity;

        [Column("shareCount")]
        public uint ShareCount { get => _shareCount; set => SetField(ref _shareCount, value); }
        private uint _shareCount;

        [Column("createdAt")]
        public DateTimeOffset CreatedAt { get => _createdAt; set => SetField(ref _createdAt, value); }
        private DateTimeOffset _createdAt;

        [Column("difficulty")]
        public uint Difficulty { get => _difficulty; set => SetField(ref _difficulty, value); }
        private uint _difficulty;

        [Column("shares")]
        public uint Shares { get => _shares; set => SetField(ref _shares, value); }
        private uint _shares;

        public static PostAccountHistoricalDbItem FromApiItem (PostAccountHistoricalItem value)
        {
            return new PostAccountHistoricalDbItem
            {
                EstCapacity = value.Ec,
                ShareCount = value.ShareCount,
                CreatedAt = value.CreatedAt,
                Difficulty = value.Difficulty,
                Shares = value.Shares
            };
        }
    }
}
