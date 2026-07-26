using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System.Linq;
using System.Collections.Generic;

namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfLogDal : ILogDal
    {
        private readonly InvestmentContext _context;
        public EfLogDal(InvestmentContext context)
        {
            _context = context;
        }

        public void Add(Log log)
        {
            _context.Logs.Add(log);
            _context.SaveChanges();
        }

        public List<Log> GetAll()
        {
            return _context.Logs.ToList();
        }

        public List<Log> GetByUserId(int userId)
        {
            return _context.Logs.Where(l=>l.UserId==userId).ToList();
        }
    }
}