using System;
using System.Threading.Tasks;
using EmployeeManagement.Application.Caching.Handlers;
using EmployeeManagement.Persistence.Cache.Keys;
using Microsoft.Extensions.Caching.Distributed;

namespace EmployeeManagement.Persistence.Cache.Handlers;

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
