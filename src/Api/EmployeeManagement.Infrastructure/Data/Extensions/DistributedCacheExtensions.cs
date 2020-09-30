using System;
using System.Collections.Generic;
using System.Linq;
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

        public static async Task AddToListAsync<T>(this IDistributedCache distributedCache, string cacheKey, T item)
        {
            if (distributedCache == null)
            {
                throw new ArgumentNullException(nameof(distributedCache));
            }

            if (cacheKey == null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            byte[] utf8Bytes = await distributedCache.GetAsync(cacheKey);

            if (utf8Bytes != null)
            {
                List<T> itemList = JsonSerializer.Deserialize<List<T>>(utf8Bytes);

                if (itemList == null)
                {
                    itemList.Add(item);

                    DistributedCacheEntryOptions options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(30));
                    utf8Bytes = JsonSerializer.SerializeToUtf8Bytes<List<T>>(itemList);
                    await distributedCache.SetAsync(cacheKey, utf8Bytes, options);
                }
            }
        }

        public static async Task AddToListAsync<T, TKey>(this IDistributedCache distributedCache, string cacheKey, T item, Func<T, TKey> orderBy)
        {
            if (distributedCache == null)
            {
                throw new ArgumentNullException(nameof(distributedCache));
            }

            if (cacheKey == null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (orderBy == null)
            {
                throw new ArgumentNullException(nameof(orderBy));
            }

            byte[] utf8Bytes = await distributedCache.GetAsync(cacheKey);

            if (utf8Bytes != null)
            {
                List<T> itemList = JsonSerializer.Deserialize<List<T>>(utf8Bytes);

                if (itemList != null)
                {
                    itemList.Add(item);
                    itemList = itemList.OrderBy(orderBy).ToList();

                    DistributedCacheEntryOptions options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(30));
                    utf8Bytes = JsonSerializer.SerializeToUtf8Bytes<List<T>>(itemList);
                    await distributedCache.SetAsync(cacheKey, utf8Bytes, options);
                }
            }
        }

        public static async Task UpdateInListAsync<T>(this IDistributedCache distributedCache, string cacheKey, T updatedItem, Predicate<T> predicate)
        {
            if (distributedCache == null)
            {
                throw new ArgumentNullException(nameof(distributedCache));
            }

            if (cacheKey == null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            if (updatedItem == null)
            {
                throw new ArgumentNullException(nameof(updatedItem));
            }

            byte[] utf8Bytes = await distributedCache.GetAsync(cacheKey);

            if (utf8Bytes != null)
            {
                List<T> itemList = JsonSerializer.Deserialize<List<T>>(utf8Bytes);

                if (itemList != null)
                {
                    int itemIndex = itemList.FindIndex(predicate);
                    itemList[itemIndex] = updatedItem;

                    DistributedCacheEntryOptions options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(30));
                    utf8Bytes = JsonSerializer.SerializeToUtf8Bytes<List<T>>(itemList);
                    await distributedCache.SetAsync(cacheKey, utf8Bytes, options);
                }
            }
        }

        public static async Task RemoveFromListAsync<T>(this IDistributedCache distributedCache, string cacheKey, Predicate<T> predicate)
        {
            if (distributedCache == null)
            {
                throw new ArgumentNullException(nameof(distributedCache));
            }

            if (cacheKey == null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            byte[] utf8Bytes = await distributedCache.GetAsync(cacheKey);

            if (utf8Bytes != null)
            {
                List<T> itemList = JsonSerializer.Deserialize<List<T>>(utf8Bytes);

                if (itemList != null)
                {
                    T itemToBeRemoved = itemList.Find(predicate);

                    if (itemToBeRemoved != null)
                    {
                        itemList.Remove(itemToBeRemoved);

                        DistributedCacheEntryOptions options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(30));
                        utf8Bytes = JsonSerializer.SerializeToUtf8Bytes<List<T>>(itemList);
                        await distributedCache.SetAsync(cacheKey, utf8Bytes, options);
                    }
                }
            }
        }
    }
}
