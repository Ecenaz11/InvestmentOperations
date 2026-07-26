using System.Collections.Generic;
using InvestmentOperations.Entities.Concrete;

namespace InvestmentOperations.DataAccess.Abstract
{
    public interface ILogDal
    {
        void Add(Log log);
        List<Log> GetAll();
        List<Log> GetByUserId(int userId);
    }
}