using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using EmployeeManagement.Infrastructure.Data.CacheKeys;
using Microsoft.Extensions.Caching.Distributed;

namespace EmployeeManagement.Infrastructure.Data.Repositories
{
    internal class DepartmentRepository : IDepartmentRepository
    {
        private readonly IRepository<Department> _repository;
        private readonly IDistributedCache _distributedCache;

        public DepartmentRepository(IRepository<Department> repository, IDistributedCache distributedCache)
        {
            _repository = repository;
            _distributedCache = distributedCache;
        }

        public IQueryable<Department> Departments => _repository.Entities;

        public async Task<Department> GetByIdAsync(long departmentId)
        {
            return await _repository.GetByIdAsync(departmentId);
        }

        public async Task<int> InsertAsync(Department department)
        {
            object[] primaryKeyValues = await _repository.InsertAsync(department);

            // Remove Cache
            string departmentListKey = DepartmentCacheKeys.ListKey;
            await _distributedCache.RemoveAsync(departmentListKey);

            string departmentSelectListKey = DepartmentCacheKeys.SelectListKey;
            await _distributedCache.RemoveAsync(departmentSelectListKey);

            return (int)primaryKeyValues[0];
        }

        public async Task UpdateAsync(Department department)
        {
            await _repository.UpdateAsync(department);

            // Remove Cache
            string departmentDetailsCacheKey = DepartmentCacheKeys.GetDetailsKey(department.DepartmentId);
            string departmentCacheKey = DepartmentCacheKeys.GetKey(department.DepartmentId);
            await _distributedCache.RemoveAsync(departmentDetailsCacheKey);
            await _distributedCache.RemoveAsync(departmentCacheKey);
        }

        public async Task DeleteAsync(Department department)
        {
            await _repository.DeleteAsync(department);

            // Remove Cache
            string departmentDetailsCacheKey = DepartmentCacheKeys.GetDetailsKey(department.DepartmentId);
            string departmentCacheKey = DepartmentCacheKeys.GetKey(department.DepartmentId);
            await _distributedCache.RemoveAsync(departmentDetailsCacheKey);
            await _distributedCache.RemoveAsync(departmentCacheKey);
        }
    }
}
