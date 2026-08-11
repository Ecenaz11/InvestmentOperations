using InvestmentOperations.Entities.Dtos;
using System.Threading.Tasks;

namespace InvestmentOperations.Business.Abstract
{
    public interface IGoldApiClient
    {
        Task<GoldApiPriceResponse> GetPriceAsync(string metalSymbol);
    }
}