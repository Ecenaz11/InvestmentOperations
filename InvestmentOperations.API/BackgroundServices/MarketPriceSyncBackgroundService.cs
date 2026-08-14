using InvestmentOperations.Business.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InvestmentOperations.API.BackgroundServices
{
    public class MarketPriceSyncBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MarketPriceSyncBackgroundService> _logger;
        private readonly TimeSpan _interval;

        public MarketPriceSyncBackgroundService(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<MarketPriceSyncBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            var hours = configuration.GetValue<double>("MarketPriceSync:IntervalHours", 48);
            _interval = TimeSpan.FromHours(hours);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var syncService = scope.ServiceProvider.GetRequiredService<IMarketPriceSyncService>();
                        await syncService.SyncPricesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Market price sync failed.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}