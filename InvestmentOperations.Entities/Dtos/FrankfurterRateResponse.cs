using System.Text.Json.Serialization;

namespace InvestmentOperations.Entities.Dtos
{
    public class FrankfurterRateResponse
    {
        [JsonPropertyName("base")]
        public string Base { get; set; }

        [JsonPropertyName("quote")]
        public string Quote { get; set; }

        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }
    }
}