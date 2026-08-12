using Microsoft.EntityFrameworkCore.Storage;
using InvestmentOperations.Core.DataAccess;

namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InvestmentContext _context;
        private  IDbContextTransaction? _transaction;
        public UnitOfWork (InvestmentContext context)
        {
            _context = context;
        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction(); 
        }

        public void Commit()
        {
            _transaction?.Commit(); 
            _transaction?.Dispose(); 
            _transaction = null;  
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}