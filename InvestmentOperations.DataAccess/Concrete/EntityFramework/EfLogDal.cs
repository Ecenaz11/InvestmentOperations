using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
        }

        public async Task<List<Log>> GetAllAsync()
        {
            return await _context.Logs.ToListAsync();
        }

        public async Task<List<Log>> GetByUserIdAsync(int userId)
        {
            return await _context.Logs.Where(l => l.UserId == userId).ToListAsync();
        }
    }
}
