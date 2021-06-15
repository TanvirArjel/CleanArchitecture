using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EmployeeManagement.Application.CacheRepositories;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Data.CacheKeys;
using Microsoft.Extensions.Caching.Distributed;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.Caching;

namespace EmployeeManagement.Infrastructure.Data.CacheRepositories
{
    internal class DepartmentCacheRepository : IDepartmentCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepository _repository;

        public DepartmentCacheRepository(IDistributedCache distributedCache, IRepository repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public async Task<List<DepartmentDetailsDto>> GetListAsync()
        {
            string cacheKey = DepartmentCacheKeys.ListKey;
            List<DepartmentDetailsDto> departmentList = await _distributedCache.GetAsync<List<DepartmentDetailsDto>>(cacheKey);

            if (departmentList == null)
            {
                Expression<Func<Department, DepartmentDetailsDto>> selectExp = d => new DepartmentDetailsDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    IsActive = d.IsActive,
                    CreatedAtUtc = d.CreatedAtUtc,
                    LastModifiedAtUtc = d.LastModifiedAtUtc
                };

                departmentList = await _repository.GetListAsync(selectExp);

                await _distributedCache.SetAsync<List<DepartmentDetailsDto>>(cacheKey, departmentList);
            }

            return departmentList;
        }

        public async Task<List<DepartmentSelectListDto>> GetSelectListAsync()
        {
            string cacheKey = DepartmentCacheKeys.SelectListKey;
            List<DepartmentSelectListDto> departmentSelectList = await _distributedCache.GetAsync<List<DepartmentSelectListDto>>(cacheKey);

            if (departmentSelectList == null)
            {
                Expression<Func<Department, DepartmentSelectListDto>> selectExp = d => new DepartmentSelectListDto
                {
                    Id = d.Id,
                    Name = d.Name,
                };

                departmentSelectList = await _repository.GetListAsync(selectExp);

                await _distributedCache.SetAsync<List<DepartmentSelectListDto>>(cacheKey, departmentSelectList);
            }

            return departmentSelectList;
        }

        public async Task<Department> GetByIdAsync(int departmentId)
        {
            string cacheKey = DepartmentCacheKeys.GetKey(departmentId);
            Department department = await _distributedCache.GetAsync<Department>(cacheKey);

            if (department == null)
            {
                department = await _repository.GetByIdAsync<Department>(departmentId);

                await _distributedCache.SetAsync<Department>(cacheKey, department);
            }

            return department;
        }

        public async Task<DepartmentDetailsDto> GetDetailsByIdAsync(int departmentId)
        {
            string cacheKey = DepartmentCacheKeys.GetDetailsKey(departmentId);
            DepartmentDetailsDto department = await _distributedCache.GetAsync<DepartmentDetailsDto>(cacheKey);

            if (department == null)
            {
                Expression<Func<Department, DepartmentDetailsDto>> selectExp = d => new DepartmentDetailsDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    IsActive = d.IsActive,
                    CreatedAtUtc = d.CreatedAtUtc,
                    LastModifiedAtUtc = d.LastModifiedAtUtc
                };

                department = await _repository.GetByIdAsync(departmentId, selectExp);

                await _distributedCache.SetAsync<DepartmentDetailsDto>(cacheKey, department);
            }

            return department;
        }

        public async Task<int> InsertAsync(Department department)
        {
            object[] primaryKeyValues = await _repository.InsertAsync(department);

            int departmentId = (int)primaryKeyValues[0];

            // Add item to the cache list
            string departmentListKey = DepartmentCacheKeys.ListKey;
            await _distributedCache.RemoveAsync(departmentListKey);

            string departmentSelectListKey = DepartmentCacheKeys.SelectListKey;
            await _distributedCache.RemoveAsync(departmentSelectListKey);

            return departmentId;
        }

        public async Task UpdateAsync(Department department)
        {
            await _repository.UpdateAsync(department);

            // Update item in cache
            string departmentCacheKey = DepartmentCacheKeys.GetKey(department.Id);
            await _distributedCache.RemoveAsync(departmentCacheKey);

            string departmentDetailsCacheKey = DepartmentCacheKeys.GetDetailsKey(department.Id);

            await _distributedCache.RemoveAsync(departmentDetailsCacheKey);

            string departmentListKey = DepartmentCacheKeys.ListKey;
            await _distributedCache.RemoveAsync(departmentListKey);

            string departmenSelecttListKey = DepartmentCacheKeys.SelectListKey;
            await _distributedCache.RemoveAsync(departmenSelecttListKey);
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
