using System.Collections.Generic;
using InvestmentOperations.Entities.Concrete;
using System.Threading.Tasks;

namespace InvestmentOperations.DataAccess.Abstract
{
    public interface ILogDal
    {
        void Add(Log log);
        Task<List<Log>> GetAllAsync();
        Task<List<Log>> GetByUserIdAsync(int userId);
    }
}
