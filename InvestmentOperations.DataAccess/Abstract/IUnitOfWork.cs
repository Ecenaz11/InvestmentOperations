namespace InvestmentOperations.DataAccess.Abstract;

public interface IUnitOfWork
{
    void SaveChanges();
    void BeginTransaction();
    void Commit();
    void Rollback();
}

