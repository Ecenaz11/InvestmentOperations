using InvestmentOperations.Business.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InvestmentOperations.API.BackgroundServices
{
    public class MarketPriceSyncBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval;

        public MarketPriceSyncBackgroundService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            var hours = configuration.GetValue<double>("MarketPriceSync:IntervalHours", 48);
            _interval = TimeSpan.FromHours(hours);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var syncService = scope.ServiceProvider.GetRequiredService<IMarketPriceSyncService>();
                    await syncService.SyncPricesAsync();
                }
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}