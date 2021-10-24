using System.Text.Json;
using System.Text.Json.Serialization;

namespace FoxyMonitor.Models.AtbResponses
{
    public class ATBChainResponse
    {
        [JsonPropertyName("netspaceBytes")]
        public decimal NetspaceBytes { get; set; }

        [JsonPropertyName("difficulty")]
        public uint Difficulty { get; set; }

        [JsonPropertyName("peakHeight")]
        public ulong PeakHeight { get; set; }

        [JsonPropertyName("uniqueAddressCount")]
        public ulong UniqueAddressCount { get; set; }

        [JsonPropertyName("totalCoinSupply")]
        public ulong TotalCoinSupply { get; set; }

        [JsonPropertyName("peakAgeSeconds")]
        public uint PeakAgeSeconds { get; set; }

        [JsonPropertyName("coinHeight")]
        public ulong CoinHeight { get; set; }

        public static ATBChainResponse FromJson(string json) => JsonSerializer.Deserialize<ATBChainResponse>(json);
    }
}
