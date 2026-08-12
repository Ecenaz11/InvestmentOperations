namespace InvestmentOperations.Core.DataAccess;

public interface IUnitOfWork
{
    void SaveChanges();
    void BeginTransaction();
    void Commit();
    void Rollback();
}

