using System;
using System.Threading.Tasks;
using CleanHr.Application.Caching.Handlers;
using CleanHr.Persistence.Cache.Keys;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanHr.Persistence.Cache.Handlers;

internal sealed class DepartmentCacheHandler(IDistributedCache distributedCache) : IDepartmentCacheHandler
{
    private readonly IDistributedCache _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));

    public async Task RemoveListAsync()
    {
        string departmentListKey = DepartmentCacheKeys.ListKey;
        await _distributedCache.RemoveAsync(departmentListKey);
    }
}
