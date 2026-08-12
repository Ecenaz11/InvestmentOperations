using System;
using System.Threading;
using System.Threading.Tasks;
using InvestmentOperations.Core.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace InvestmentOperations.DataAccess.Concrete.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory,
            TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            if (_memoryCache.TryGetValue(key, out T cached))
            {
                return cached;
            }

            var value = await factory(cancellationToken);

            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }

            _memoryCache.Set(key, value, options);
            return value;
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }
    }
}