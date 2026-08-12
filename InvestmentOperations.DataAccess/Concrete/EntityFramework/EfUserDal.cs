using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;

namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfUserDal : EfEntityRepositoryBase<User, InvestmentContext>, IUserDal
    {
        public EfUserDal(InvestmentContext context) : base(context) { }
    }
}