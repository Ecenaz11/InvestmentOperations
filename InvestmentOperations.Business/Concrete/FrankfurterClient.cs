using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Dtos;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace InvestmentOperations.Business.Concrete
{
    public class FrankfurterClient : IFrankfurterClient
    {
        private readonly HttpClient _httpClient;

        public FrankfurterClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<FrankfurterRateResponse>> GetRateAsync(string baseCurrency)
        {
            return await _httpClient.GetFromJsonAsync<List<FrankfurterRateResponse>>($"rates/latest?base={baseCurrency}&quotes=TRY");
        }
    }
}