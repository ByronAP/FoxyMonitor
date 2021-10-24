using System.Text.Json.Serialization;

namespace FoxyMonitor.Models.AtbResponses
{
    public class AtbSort
    {
        [JsonPropertyName("empty")]
        public bool Empty { get; set; }

        [JsonPropertyName("sorted")]
        public bool Sorted { get; set; }

        [JsonPropertyName("unsorted")]
        public bool Unsorted { get; set; }
    }
}
