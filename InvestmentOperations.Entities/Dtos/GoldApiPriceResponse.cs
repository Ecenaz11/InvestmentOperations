using System.Text.Json.Serialization;

namespace InvestmentOperations.Entities.Dtos
{
    public class GoldApiPriceResponse
    {
        [JsonPropertyName("metal")]
        public string Metal { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("price_gram_24k")]
        public decimal PriceGram24k { get; set; }
    }
}