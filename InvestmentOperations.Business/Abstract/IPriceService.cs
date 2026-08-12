using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using InvestmentOperations.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.Business.Abstract
{
    public interface IPriceService
    {
        Task<IResult> Add(Price price);
        Task<IResult> Delete(int id);
        Task<IDataResult<PriceDto>> GetById(int id);
        Task<IDataResult<List<PriceDto>>> GetAll();
        Task<IDataResult<Price>> GetByAssetId(int assetId);
        Task<IResult> Update(Price price);
    }
}
