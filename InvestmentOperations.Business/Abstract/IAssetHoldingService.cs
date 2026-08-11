using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using InvestmentOperations.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.Business.Abstract
{
    public interface IAssetHoldingService
    {
        IDataResult<List<AssetHolding>> GetByUserId(int userId);
        IDataResult<List<AssetHoldingDto>> GetAllDetailed();
        IDataResult<AssetHoldingDto> GetByIdDetailed(int id);
        IDataResult<List<AssetHoldingDto>> GetByUserIdDetailed(int userId);
        IResult Add(AssetHolding assetHolding);
        IResult Deposit(int userId, decimal amount);
        IResult Update(AssetHolding assetHolding);
        IResult Delete(int id);
    }
}
