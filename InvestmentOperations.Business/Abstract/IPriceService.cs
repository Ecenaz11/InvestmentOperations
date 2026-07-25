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
        IResult Add(Price price);
        IResult Update(Price price);
        IResult Delete(int id);
        IDataResult<PriceDto> GetById(int id);
        IDataResult<List<PriceDto>> GetAll();
        IDataResult<Price> GetByAssetId(int assetId);
    }
}
