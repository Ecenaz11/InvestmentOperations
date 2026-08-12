using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;

namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfAssetHoldingDal : EfEntityRepositoryBase<AssetHolding, InvestmentContext>, IAssetHoldingDal
    {
        public EfAssetHoldingDal(InvestmentContext context) : base(context) { }
    }
}