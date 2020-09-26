using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Domain.CacheRepositories;
using EmployeeManagement.Domain.Dtos.DepartmentDtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Data.CacheKeys;
using EmployeeManagement.Infrastructure.Data.Extensions;
using EmployeeManagement.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace EmployeeManagement.Infrastructure.Data.CacheRepositories
{
    internal class DepartmentCacheRepository : IDepartmentCacheRepository
    {
        private readonly IRepository<Department> _repository;
        private readonly IDistributedCache _distributedCache;

        public DepartmentCacheRepository(IRepository<Department> repository, IDistributedCache distributedCache)
        {
            _repository = repository;
            _distributedCache = distributedCache;
        }

        public async Task<Department> GetByIdAsync(int departmentId)
        {
            string cacheKey = DepartmentCacheKeys.GetKey(departmentId);
            Department department = await _distributedCache.GetAsync<Department>(cacheKey);

            if (department == null)
            {
                department = await _repository.Entities.AsNoTracking().FirstOrDefaultAsync(e => e.DepartmentId == departmentId);

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
                department = await _repository.Entities.Where(d => d.DepartmentId == departmentId)
                .Select(d => new DepartmentDetailsDto
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    Description = d.Description,
                    IsActive = d.IsActive,
                    CreatedAtUtc = d.CreatedAtUtc,
                    LastModifiedAtUtc = d.LastModifiedAtUtc
                }).FirstOrDefaultAsync();

                await _distributedCache.SetAsync<DepartmentDetailsDto>(cacheKey, department);
            }

            return department;
        }

        public async Task<List<DepartmentDetailsDto>> GetListAsync()
        {
            string cacheKey = DepartmentCacheKeys.ListKey;
            List<DepartmentDetailsDto> departmentList = await _distributedCache.GetAsync<List<DepartmentDetailsDto>>(cacheKey);

            if (departmentList == null)
            {
                departmentList = await _repository.Entities.Select(d => new DepartmentDetailsDto
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    Description = d.Description,
                    IsActive = d.IsActive,
                    CreatedAtUtc = d.CreatedAtUtc,
                    LastModifiedAtUtc = d.LastModifiedAtUtc
                }).ToListAsync();

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
                departmentSelectList = await _repository.Entities.Select(d => new DepartmentSelectListDto
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                }).ToListAsync();

                await _distributedCache.SetAsync<List<DepartmentSelectListDto>>(cacheKey, departmentSelectList);
            }

            return departmentSelectList;
        }
    }
}
