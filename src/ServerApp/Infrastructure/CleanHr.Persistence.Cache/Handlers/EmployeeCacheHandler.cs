using System;
using System.Threading.Tasks;
using CleanHr.Application.Caching.Handlers;
using CleanHr.Persistence.Cache.Keys;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanHr.Persistence.Cache.Handlers;

internal class EmployeeCacheHandler : IEmployeeCacheHandler
{
    private readonly IDistributedCache _distributedCache;

    public EmployeeCacheHandler(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task RemoveDetailsByIdAsync(Guid employeeId)
    {
        string detailsKey = EmployeeCacheKeys.GetDetailsKey(employeeId);
        await _distributedCache.RemoveAsync(detailsKey);
    }
}
