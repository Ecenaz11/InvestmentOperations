using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using System.Collections.Generic;

namespace InvestmentOperations.Business.Abstract
{
    public interface ILogService
    {
        IResult Add(Log log);
        IDataResult<List<Log>> GetAll();
        IDataResult<List<Log>> GetByUserId(int userId);
    }
}