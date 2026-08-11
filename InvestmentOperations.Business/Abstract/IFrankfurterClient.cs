using InvestmentOperations.Entities.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvestmentOperations.Business.Abstract
{
    public interface IFrankfurterClient
    {
        Task<List<FrankfurterRateResponse>> GetRateAsync(string baseCurrency);
    }
}