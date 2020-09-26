using System.Threading.Tasks;
using EmployeeManagement.Domain.CacheRepositories;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Data.Extensions;
using EmployeeManagement.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace EmployeeManagement.Infrastructure.Data.CacheRepositories
{
    internal class EmployeeCacheRepository : IEmployeeCacheRepository
    {
        private readonly IRepository<Employee> _repository;
        private readonly IDistributedCache _distributedCache;

        public EmployeeCacheRepository(IRepository<Employee> repository, IDistributedCache distributedCache)
        {
            _repository = repository;
            _distributedCache = distributedCache;
        }

        public async Task<Employee> GetByIdAsync(long employeeId)
        {
            string cacheKey = $"EmployeeId{employeeId}";
            Employee employee = await _distributedCache.GetAsync<Employee>(cacheKey);

            if (employee == null)
            {
                employee = await _repository.Entities.AsNoTracking().FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

                await _distributedCache.SetAsync<Employee>(cacheKey, employee);
            }

            return employee;
        }
    }
}
