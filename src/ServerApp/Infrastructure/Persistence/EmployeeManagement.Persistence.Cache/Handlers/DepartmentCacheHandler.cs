using System.Threading.Tasks;
using EmployeeManagement.Application.Caching.Handlers;
using EmployeeManagement.Persistence.Cache.Keys;
using Microsoft.Extensions.Caching.Distributed;

namespace EmployeeManagement.Persistence.Cache.Handlers
{
    internal class DepartmentCacheHandler : IDepartmentCacheHandler
    {
        private readonly IDistributedCache _distributedCache;

        public DepartmentCacheHandler(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task RemoveListCacheAsync()
        {
            string departmentListKey = DepartmentCacheKeys.ListKey;
            await _distributedCache.RemoveAsync(departmentListKey);
        }
    }
}
