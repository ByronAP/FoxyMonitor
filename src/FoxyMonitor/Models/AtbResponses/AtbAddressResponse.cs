using System.Text.Json;
using System.Text.Json.Serialization;

namespace FoxyMonitor.Models.AtbResponses
{
    public class AtbAddressResponse
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("puzzleHash")]
        public string PuzzleHash { get; set; }

        [JsonPropertyName("balance")]
        public ulong Balance { get; set; }

        [JsonPropertyName("balanceBefore")]
        public ulong BalanceBefore { get; set; }

        [JsonPropertyName("timestampBalanceBefore")]
        public ulong TimestampBalanceBefore { get; set; }

        [JsonPropertyName("createTimestamp")]
        public ulong CreateTimestamp { get; set; }

        [JsonPropertyName("addressType")]
        public string AddressType { get; set; }

        public static AtbAddressResponse FromJson(string json) => JsonSerializer.Deserialize<AtbAddressResponse>(json);
    }
}
