using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvestmentOperations.Business.Abstract
{
    public interface ILogService
    {
        IResult Add(Log log);
        Task<IDataResult<List<Log>>> GetAll();
        Task<IDataResult<List<Log>>> GetByUserId(int userId);
    }
}
