using InvestmentOperations.DataAccess.Abstract;

namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InvestmentContext _context;
        public UnitOfWork (InvestmentContext context)
        {
            _context = context;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}