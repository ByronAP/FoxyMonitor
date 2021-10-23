using FoxyPoolApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FoxyMonitor.Models
{
    [Index(nameof(PoolName), nameof(LauncherId), nameof(Timestamp), IsUnique = true)]
    public class AccountBalanceHistoricalDbItem : ObservableObject
    {
        [Key]
        public uint Id { get => _id; set => SetProperty(ref _id, value); }
        private uint _id;

        public string PoolName { get => _poolName; set => SetProperty(ref _poolName, value); }
        private string _poolName = PostPool.Chia_OG.ToString();

        public string LauncherId { get => _launcerId; set => SetProperty(ref _launcerId, value); }
        private string _launcerId = string.Empty;

        public ulong Timestamp { get => _timestamp; set => SetProperty(ref _timestamp, value); }
        private ulong _timestamp;

        public ulong Balance { get => _balance; set => SetProperty(ref _balance, value); }
        private ulong _balance;
    }
}
