using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using InvestmentOperations.Entities.Dtos;


namespace InvestmentOperations.Business.Abstract
{
    public interface ITradeService
    {
        IResult Add(Trade trade);
        IResult Update(Trade trade);
        IResult Delete(int id);
       IDataResult<List<TradeDto>>GetAll();
        IDataResult<TradeDto> GetById(int id);
    }
}
