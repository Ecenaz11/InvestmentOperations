using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using InvestmentOperations.Entities.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvestmentOperations.Business.Abstract
{
    public interface IAssetHoldingService
    {
        Task<IDataResult<List<AssetHolding>>> GetByUserId(int userId);
        Task<IDataResult<List<AssetHoldingDto>>> GetAllDetailed();
        Task<IDataResult<AssetHoldingDto>> GetByIdDetailed(int id);
        Task<IDataResult<List<AssetHoldingDto>>> GetByUserIdDetailed(int userId);
        Task<IResult> Add(AssetHolding assetHolding);
        Task<IResult> Deposit(int userId, decimal amount);
        Task<IResult> Update(AssetHolding assetHolding);
        Task<IResult> Delete(int id);
    }
}
