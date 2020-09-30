using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Dtos.DepartmentDtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using EmployeeManagement.Infrastructure.Data.CacheKeys;
using EmployeeManagement.Infrastructure.Data.Extensions;
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

            int departmentId = (int)primaryKeyValues[0];
            department.DepartmentId = departmentId;

            // Add item to the cache list
            string departmentCacheKey = DepartmentCacheKeys.GetKey(department.DepartmentId);
            await _distributedCache.SetAsync(departmentCacheKey, department);

            string departmentDetailsCacheKey = DepartmentCacheKeys.GetDetailsKey(department.DepartmentId);

            DepartmentDetailsDto departmentDetailsDto = new DepartmentDetailsDto()
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                Description = department.Description,
                IsActive = department.IsActive,
                CreatedAtUtc = department.CreatedAtUtc,
                LastModifiedAtUtc = department.LastModifiedAtUtc
            };
            await _distributedCache.SetAsync(departmentDetailsCacheKey, departmentDetailsDto);

            string departmentListKey = DepartmentCacheKeys.ListKey;
            await _distributedCache.AddToListAsync(departmentListKey, department, d => d.DepartmentName);

            string departmentSelectListKey = DepartmentCacheKeys.SelectListKey;
            await _distributedCache.AddToListAsync(departmentSelectListKey, department, d => d.DepartmentName);

            return departmentId;
        }

        public async Task UpdateAsync(Department department)
        {
            await _repository.UpdateAsync(department);

            // Update item in cache
            string departmentCacheKey = DepartmentCacheKeys.GetKey(department.DepartmentId);
            await _distributedCache.SetAsync<Department>(departmentCacheKey, department);

            string departmentDetailsCacheKey = DepartmentCacheKeys.GetDetailsKey(department.DepartmentId);

            DepartmentDetailsDto departmentDetailsDto = new DepartmentDetailsDto()
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                Description = department.Description,
                IsActive = department.IsActive,
                CreatedAtUtc = department.CreatedAtUtc,
                LastModifiedAtUtc = department.LastModifiedAtUtc
            };
            await _distributedCache.SetAsync(departmentDetailsCacheKey, departmentDetailsDto);

            string departmentListKey = DepartmentCacheKeys.ListKey;
            await _distributedCache.UpdateInListAsync(departmentListKey, department, d => d.DepartmentId == department.DepartmentId);

            string departmenSelecttListKey = DepartmentCacheKeys.SelectListKey;
            await _distributedCache.UpdateInListAsync(departmenSelecttListKey, department, d => d.DepartmentId == department.DepartmentId);
        }

        public async Task DeleteAsync(Department department)
        {
            await _repository.DeleteAsync(department);

            // Remove item from cache
            string cacheKey = DepartmentCacheKeys.GetKey(department.DepartmentId);
            await _distributedCache.RemoveAsync(cacheKey);

            string departmentDetailsCacheKey = DepartmentCacheKeys.GetDetailsKey(department.DepartmentId);
            await _distributedCache.RemoveAsync(departmentDetailsCacheKey);

            string departmentListKey = DepartmentCacheKeys.ListKey;
            await _distributedCache.RemoveFromListAsync<Department>(departmentListKey, d => d.DepartmentId == department.DepartmentId);

            string departmentSelectListKey = DepartmentCacheKeys.SelectListKey;
            await _distributedCache.RemoveFromListAsync<Department>(departmentSelectListKey, d => d.DepartmentId == department.DepartmentId);
        }
    }
}
