using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Dtos;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace InvestmentOperations.Business.Concrete
{
    public class GoldApiClient : IGoldApiClient
    {
        private readonly HttpClient _httpClient;

        public GoldApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<GoldApiPriceResponse> GetPriceAsync(string metalSymbol)
        {
            return await _httpClient.GetFromJsonAsync<GoldApiPriceResponse>($"{metalSymbol}/TRY");
        }
    }
}