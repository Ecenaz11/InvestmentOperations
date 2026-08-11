using System.Threading.Tasks;

namespace InvestmentOperations.Business.Abstract
{
    public interface IMarketPriceSyncService
    {
        Task SyncPricesAsync();
    }
}