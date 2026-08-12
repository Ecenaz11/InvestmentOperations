using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;

namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfPriceDal : EfEntityRepositoryBase<Price, InvestmentContext>, IPriceDal
    {
        public EfPriceDal(InvestmentContext context) : base(context) { }
    }
}