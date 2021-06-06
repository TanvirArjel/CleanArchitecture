using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Dtos.DepartmentDtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using EmployeeManagement.Infrastructure.Data.CacheKeys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.Caching;

namespace EmployeeManagement.Infrastructure.Data.Repositories
{
    internal class DepartmentRepository : IDepartmentRepository
    {
        private readonly IRepository _repository;
        private readonly IDistributedCache _distributedCache;

        public DepartmentRepository(IRepository repository, IDistributedCache distributedCache)
        {
            _repository = repository;
            _distributedCache = distributedCache;
        }

        public IQueryable<Department> Departments => _repository.GetQueryable<Department>();

        public async Task<List<DepartmentDetailsDto>> GetListAsync()
        {
            List<DepartmentDetailsDto> departmentList = await Departments.Select(d => new DepartmentDetailsDto
            {
                DepartmentId = d.Id,
                DepartmentName = d.Name,
                Description = d.Description,
                IsActive = d.IsActive,
                CreatedAtUtc = d.CreatedAtUtc,
                LastModifiedAtUtc = d.LastModifiedAtUtc
            }).ToListAsync();

            return departmentList;
        }

        public async Task<List<DepartmentSelectListDto>> GetSelectListAsync()
        {
            List<DepartmentSelectListDto> selectList = await Departments.Select(d => new DepartmentSelectListDto
            {
                DepartmentId = d.Id,
                DepartmentName = d.Name,
            }).ToListAsync();

            return selectList;
        }

        public async Task<Department> GetByIdAsync(int departmentId)
        {
            return await _repository.GetByIdAsync<Department>(departmentId);
        }

        public async Task<DepartmentDetailsDto> GetDetailsByIdAsync(int departmentId)
        {
            DepartmentDetailsDto departmentDetails = await Departments.Where(d => d.Id == departmentId)
                .Select(d => new DepartmentDetailsDto
                {
                    DepartmentId = d.Id,
                    DepartmentName = d.Name,
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
            department.Id = departmentId;

            // Add item to the cache list
            string departmentCacheKey = DepartmentCacheKeys.GetKey(department.Id);
            await _distributedCache.SetAsync(departmentCacheKey, department);

            string departmentDetailsCacheKey = DepartmentCacheKeys.GetDetailsKey(department.Id);

            DepartmentDetailsDto departmentDetailsDto = new DepartmentDetailsDto()
            {
                DepartmentId = department.Id,
                DepartmentName = department.Name,
                Description = department.Description,
                IsActive = department.IsActive,
                CreatedAtUtc = department.CreatedAtUtc,
                LastModifiedAtUtc = department.LastModifiedAtUtc
            };
            await _distributedCache.SetAsync(departmentDetailsCacheKey, departmentDetailsDto);

            string departmentListKey = DepartmentCacheKeys.ListKey;
            await _distributedCache.AddToListAsync(departmentListKey, department, d => d.Name);

            string departmentSelectListKey = DepartmentCacheKeys.SelectListKey;
            await _distributedCache.AddToListAsync(departmentSelectListKey, department, d => d.Name);

            return departmentId;
        }

        public async Task UpdateAsync(Department department)
        {
            await _repository.UpdateAsync(department);

            // Update item in cache
            string departmentCacheKey = DepartmentCacheKeys.GetKey(department.Id);
            await _distributedCache.SetAsync<Department>(departmentCacheKey, department);

            string departmentDetailsCacheKey = DepartmentCacheKeys.GetDetailsKey(department.Id);

            DepartmentDetailsDto departmentDetailsDto = new DepartmentDetailsDto()
            {
                DepartmentId = department.Id,
                DepartmentName = department.Name,
                Description = department.Description,
                IsActive = department.IsActive,
                CreatedAtUtc = department.CreatedAtUtc,
                LastModifiedAtUtc = department.LastModifiedAtUtc
            };

            await _distributedCache.SetAsync(departmentDetailsCacheKey, departmentDetailsDto);

            string departmentListKey = DepartmentCacheKeys.ListKey;
            await _distributedCache.UpdateInListAsync<Department>(departmentListKey, d => d.Id == department.Id, department);

            string departmenSelecttListKey = DepartmentCacheKeys.SelectListKey;
            await _distributedCache.UpdateInListAsync(departmenSelecttListKey, d => d.Id == department.Id, department);
        }

        public async Task DeleteAsync(Department department)
        {
            await _repository.DeleteAsync(department);

            // Remove item from cache
            string cacheKey = DepartmentCacheKeys.GetKey(department.Id);
            await _distributedCache.RemoveAsync(cacheKey);

            string departmentDetailsCacheKey = DepartmentCacheKeys.GetDetailsKey(department.Id);
            await _distributedCache.RemoveAsync(departmentDetailsCacheKey);

            string departmentListKey = DepartmentCacheKeys.ListKey;
            await _distributedCache.RemoveFromListAsync<Department>(departmentListKey, d => d.Id == department.Id);

            string departmentSelectListKey = DepartmentCacheKeys.SelectListKey;
            await _distributedCache.RemoveFromListAsync<Department>(departmentSelectListKey, d => d.Id == department.Id);
        }
    }
}
