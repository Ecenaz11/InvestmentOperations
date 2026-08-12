using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using System.Collections.Generic;
using InvestmentOperations.Entities.Dtos;
using System.Threading.Tasks;

namespace InvestmentOperations.Business.Abstract
{
    public interface ITradeService
    {
        Task<IResult> Add(Trade trade);
        Task<IDataResult<List<TradeDto>>> GetAll();
        Task<IDataResult<TradeDto>> GetById(int id);
        Task<IDataResult<List<TradeDto>>> GetByUserId(int userId);
    }
}
