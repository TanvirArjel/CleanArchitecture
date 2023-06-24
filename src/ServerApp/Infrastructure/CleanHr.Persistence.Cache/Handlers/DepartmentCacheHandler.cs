using System.Threading.Tasks;
using CleanHr.Application.Caching.Handlers;
using CleanHr.Persistence.Cache.Keys;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanHr.Persistence.Cache.Handlers;

internal sealed class DepartmentCacheHandler : IDepartmentCacheHandler
{
    private readonly IDistributedCache _distributedCache;

    public DepartmentCacheHandler(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task RemoveListAsync()
    {
        string departmentListKey = DepartmentCacheKeys.ListKey;
        await _distributedCache.RemoveAsync(departmentListKey);
    }
}
