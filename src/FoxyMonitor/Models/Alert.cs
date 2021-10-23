using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace FoxyMonitor.Models
{
    [Index(nameof(AccountId), nameof(AlertType), IsUnique = true)]
    public class Alert : ObservableObject
    {
        [Key]
        public uint Id { get => _id; set => SetProperty(ref _id, value); }
        private uint _id;

        public uint AccountId { get => _accountId; set => SetProperty(ref _accountId, value); }
        private uint _accountId;

        public AlertType AlertType { get => _alertType; set => SetProperty(ref _alertType, value); }
        private AlertType _alertType = AlertType.Update;

        public LogLevel Level { get => _level; set => SetProperty(ref _level, value); }
        private LogLevel _level = LogLevel.Information;

        public string Title { get => _title; set => SetProperty(ref _title, value); }
        private string _title = string.Empty;

        public string Message { get => _message; set => SetProperty(ref _message, value); }
        private string _message = string.Empty;

        public string Url { get => _url; set => SetProperty(ref _url, value); }
        private string _url = string.Empty;

        /// <summary>
        /// Gets or sets the created (USE UTC ONLY, ms).
        /// </summary>
        /// <value>The created.</value>
        public ulong Created { get => _created; set => SetProperty(ref _created, value); }
        private ulong _created = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
