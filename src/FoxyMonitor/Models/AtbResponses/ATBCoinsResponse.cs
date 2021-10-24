using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FoxyMonitor.Models.AtbResponses
{
    public class ATBCoinsResponse
    {
        [JsonPropertyName("content")]
        public List<AtbCoinItem> Coins { get; set; }

        [JsonPropertyName("pageable")]
        public AtbPageable Pageable { get; set; }

        [JsonPropertyName("totalElements")]
        public uint TotalElements { get; set; }

        [JsonPropertyName("totalPages")]
        public uint TotalPages { get; set; }

        [JsonPropertyName("last")]
        public bool Last { get; set; }

        [JsonPropertyName("size")]
        public uint Size { get; set; }

        [JsonPropertyName("number")]
        public uint Number { get; set; }

        [JsonPropertyName("sort")]
        public AtbSort Sort { get; set; }

        [JsonPropertyName("numberOfElements")]
        public uint NumberOfElements { get; set; }

        [JsonPropertyName("first")]
        public bool First { get; set; }

        [JsonPropertyName("empty")]
        public bool Empty { get; set; }
    }
}
