using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace EmployeeManagement.Infrastructure.Data.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key)
        {
            if (distributedCache == null)
            {
                throw new ArgumentNullException(nameof(distributedCache));
            }

            byte[] utf8Bytes = await distributedCache.GetAsync(key);

            if (utf8Bytes != null)
            {
                T t = JsonSerializer.Deserialize<T>(utf8Bytes);
                return t;
            }

            return default;
        }

        public static async Task SetAsync<T>(this IDistributedCache distributedCache, string cacheKey, T obj, TimeSpan offset)
        {
            if (distributedCache == null)
            {
                throw new ArgumentNullException(nameof(distributedCache));
            }

            if (cacheKey == null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
             .SetSlidingExpiration(offset);
            byte[] utf8Bytes = JsonSerializer.SerializeToUtf8Bytes<T>(obj);
            await distributedCache.SetAsync(cacheKey, utf8Bytes, options);
        }

        public static async Task SetAsync<T>(this IDistributedCache distributedCache, string cacheKey, T obj)
        {
            if (distributedCache == null)
            {
                throw new ArgumentNullException(nameof(distributedCache));
            }

            if (cacheKey == null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
             .SetSlidingExpiration(TimeSpan.FromDays(30));
            byte[] utf8Bytes = JsonSerializer.SerializeToUtf8Bytes<T>(obj);
            await distributedCache.SetAsync(cacheKey, utf8Bytes, options);
        }
    }
}
