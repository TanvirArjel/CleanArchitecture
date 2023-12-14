using System;
using System.Threading.Tasks;
using CleanHr.Application.Caching.Handlers;
using CleanHr.Persistence.Cache.Keys;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanHr.Persistence.Cache.Handlers;

internal sealed class EmployeeCacheHandler(IDistributedCache distributedCache) : IEmployeeCacheHandler
{
    public async Task RemoveDetailsByIdAsync(Guid employeeId)
    {
        string detailsKey = EmployeeCacheKeys.GetDetailsKey(employeeId);
        await distributedCache.RemoveAsync(detailsKey);
    }
}
