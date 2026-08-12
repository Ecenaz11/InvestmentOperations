using System;
using System.Threading;
using System.Threading.Tasks;

namespace InvestmentOperations.Core.Caching
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(
            string key,
            Func<CancellationToken, Task<T>> factory,
            TimeSpan? expiration = null,
            CancellationToken cancellationToken = default);

        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}