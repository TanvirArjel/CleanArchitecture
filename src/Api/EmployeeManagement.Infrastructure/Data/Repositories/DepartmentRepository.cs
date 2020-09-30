using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Dtos.DepartmentDtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using EmployeeManagement.Infrastructure.Data.CacheKeys;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<DepartmentDetailsDto>> GetListAsync()
        {
            List<DepartmentDetailsDto> departmentList = await _repository.Entities.Select(d => new DepartmentDetailsDto
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.DepartmentName,
                Description = d.Description,
                IsActive = d.IsActive,
                CreatedAtUtc = d.CreatedAtUtc,
                LastModifiedAtUtc = d.LastModifiedAtUtc
            }).ToListAsync();

            return departmentList;
        }

        public async Task<List<DepartmentSelectListDto>> GetSelectListAsync()
        {
            List<DepartmentSelectListDto> selectList = await _repository.Entities.Select(d => new DepartmentSelectListDto
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.DepartmentName,
            }).ToListAsync();

            return selectList;
        }

        public async Task<Department> GetByIdAsync(int departmentId)
        {
            return await _repository.GetByIdAsync(departmentId);
        }

        public async Task<DepartmentDetailsDto> GetDetailsByIdAsync(int departmentId)
        {
            DepartmentDetailsDto departmentDetails = await _repository.Entities.Where(d => d.DepartmentId == departmentId)
                .Select(d => new DepartmentDetailsDto
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    Description = d.Description,
                    IsActive = d.IsActive,
                    CreatedAtUtc = d.CreatedAtUtc,
                    LastModifiedAtUtc = d.LastModifiedAtUtc
                }).FirstOrDefaultAsync();

            return departmentDetails;
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
            await RemoveCacheAsync(department.DepartmentId);
        }

        public async Task DeleteAsync(Department department)
        {
            await _repository.DeleteAsync(department);

            // Remove Cache
            await RemoveCacheAsync(department.DepartmentId);
        }

        private async Task RemoveCacheAsync(int departmentId)
        {
            string departmentDetailsCacheKey = DepartmentCacheKeys.GetDetailsKey(departmentId);
            string departmentCacheKey = DepartmentCacheKeys.GetKey(departmentId);
            await _distributedCache.RemoveAsync(departmentDetailsCacheKey);
            await _distributedCache.RemoveAsync(departmentCacheKey);

            string departmentListKey = DepartmentCacheKeys.ListKey;
            await _distributedCache.RemoveAsync(departmentListKey);

            string departmentSelectListKey = DepartmentCacheKeys.SelectListKey;
            await _distributedCache.RemoveAsync(departmentSelectListKey);
        }
    }
}
