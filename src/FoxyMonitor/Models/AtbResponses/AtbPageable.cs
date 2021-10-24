using System.Text.Json.Serialization;

namespace FoxyMonitor.Models.AtbResponses
{
    public class AtbPageable
    {
        [JsonPropertyName("sort")]
        public AtbSort Sort { get; set; }

        [JsonPropertyName("offset")]
        public uint Offset { get; set; }

        [JsonPropertyName("pageNumber")]
        public uint PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public uint PageSize { get; set; }

        [JsonPropertyName("paged")]
        public bool Paged { get; set; }

        [JsonPropertyName("unpaged")]
        public bool Unpaged { get; set; }
    }
}
