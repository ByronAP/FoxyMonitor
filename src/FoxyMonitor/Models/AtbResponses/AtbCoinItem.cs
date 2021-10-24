using System.Text.Json.Serialization;

namespace FoxyMonitor.Models.AtbResponses
{
    public class AtbCoinItem
    {
        [JsonPropertyName("coinName")]
        public string CoinName { get; set; }

        [JsonPropertyName("confirmedIndex")]
        public ulong ConfirmedIndex { get; set; }

        [JsonPropertyName("spentIndex")]
        public ulong SpentIndex { get; set; }

        [JsonPropertyName("spent")]
        public bool Spent { get; set; }

        [JsonPropertyName("coinbase")]
        public bool Coinbase { get; set; }

        [JsonPropertyName("puzzleHash")]
        public string PuzzleHash { get; set; }

        [JsonPropertyName("decodedPuzzleHash")]
        public string DecodedPuzzleHash { get; set; }

        [JsonPropertyName("coinParent")]
        public string CoinParent { get; set; }

        [JsonPropertyName("amount")]
        public ulong Amount { get; set; }

        [JsonPropertyName("timestamp")]
        public ulong Timestamp { get; set; }

        [JsonPropertyName("timestampSpent")]
        public ulong TimestampSpent { get; set; }

        [JsonPropertyName("coinType")]
        public string CoinType { get; set; }
    }
}
