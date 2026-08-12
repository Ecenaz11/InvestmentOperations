using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;

namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfTradeDal : EfEntityRepositoryBase<Trade, InvestmentContext>, ITradeDal
    {
        public EfTradeDal(InvestmentContext context) : base(context) { }
    }
}