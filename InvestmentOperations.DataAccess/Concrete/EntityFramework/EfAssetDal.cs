using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;

namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfAssetDal : EfEntityRepositoryBase<Asset, InvestmentContext>, IAssetDal
    {
        public EfAssetDal(InvestmentContext context) : base(context) { }
    }
}