using InvestmentOperation.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.Business.Abstract
{
    public interface ITradeService
    {
        IResult Add(Trade trade);
        IResult Update(Trade trade);
        IResult Delete(int id);
        IDataResult<List<Trade>> GetAll();
        IDataResult<Trade> GetById(int id);


    }
}
