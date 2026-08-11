using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Entities.Concrete;
using System.Linq;
using System.Threading.Tasks;

namespace InvestmentOperations.Business.Concrete
{
    public class MarketPriceSyncService : IMarketPriceSyncService
    {
        private readonly IAssetService _assetService;
        private readonly IPriceService _priceService;
        private readonly IGoldApiClient _goldApiClient;
        private readonly IFrankfurterClient _frankfurterClient;

        public MarketPriceSyncService(IAssetService assetService, IPriceService priceService, IGoldApiClient goldApiClient, IFrankfurterClient frankfurterClient)
        {
            _assetService = assetService;
            _priceService = priceService;
            _goldApiClient = goldApiClient;
            _frankfurterClient = frankfurterClient;
        }

        public async Task SyncPricesAsync()
        {
            var assets = _assetService.GetAll().Data;

            foreach (var asset in assets)
            {
                decimal? newPrice = null;

                if (asset.AssetType == "PRECIOUSMETAL")
                {
                    var goldResult = await _goldApiClient.GetPriceAsync(asset.AssetCode);
                    newPrice = goldResult.PriceGram24k;
                }
                else if (asset.AssetType == "CURRENCY" && asset.AssetCode != "TL")
                {
                    var rates = await _frankfurterClient.GetRateAsync(asset.AssetCode);
                    newPrice = rates.FirstOrDefault()?.Rate;
                }

                if (newPrice == null)
                {
                    continue;
                }

                UpsertPrice(asset.AssetId, newPrice.Value);
            }
        }

        private void UpsertPrice(int assetId, decimal currentPrice)
        {
            var existing = _priceService.GetByAssetId(assetId);
            if (existing.Success)
            {
                _priceService.Update(new Price
                {
                    PriceId = existing.Data.PriceId,
                    AssetId = assetId,
                    CurrentPrice = currentPrice
                });
            }
            else
            {
                _priceService.Add(new Price
                {
                    AssetId = assetId,
                    CurrentPrice = currentPrice
                });
            }
        }
    }
}
