using FoxyMonitor.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoxyMonitor.Data.Models
{
    [Table("alerts")]
    [Index(nameof(AccountId), IsUnique = false)]
    [Index(nameof(PendingDeletion), IsUnique = false)]
    [Index(nameof(Viewed), IsUnique = false)]
    public class Alert : NotifyChangeBase
    {
        [Key]
        public uint Id { get; set; }

        [Required]
        [Column("accountId")]
        public uint AccountId { get => _accountId; set => SetField(ref _accountId, value); }
        private uint _accountId;

        [Required]
        [Column("level")]
        public LogLevel Level { get => _level; set => SetField(ref _level, value); }
        private LogLevel _level = LogLevel.Information;

        [Required]
        [Column("title")]
        public string Title { get => _title; set => SetField(ref _title, value); }
        private string _title = string.Empty;

        [Required]
        [Column("message")]
        public string Message { get => _message; set => SetField(ref _message, value); }
        private string _message = string.Empty;

        [Column("url")]
        public string Url { get => _url; set => SetField(ref _url, value); }
        private string _url = string.Empty;

        [Required]
        [Column("created")]
        public DateTimeOffset Created { get => _created; set => SetField(ref _created, value); }
        private DateTimeOffset _created = DateTimeOffset.UtcNow;

        [Required]
        [Column("viewed")]
        public bool Viewed { get => _viewed; set => SetField(ref _viewed, value); }
        private bool _viewed;

        [Required]
        [Column("pendingDeletion")]
        public bool PendingDeletion { get => _pendingDeletion; set => SetField(ref _pendingDeletion, value); }
        private bool _pendingDeletion;
    }
}
