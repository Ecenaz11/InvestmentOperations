using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using InvestmentOperations.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.Business.Abstract
{
    public interface IBalanceService
    {
        IDataResult<List<Balance>> GetByUserId(int userId);
        IDataResult<List<BalanceDto>> GetAllDetailed();
        IDataResult<BalanceDto> GetByIdDetailed(int id);
        IDataResult<List<BalanceDto>> GetByUserIdDetailed(int userId);
        IResult Add(Balance balance);
        IResult Deposit(int userId, decimal amount);
        IResult Update(Balance balance);
        IResult Delete(int id);
    }
}
