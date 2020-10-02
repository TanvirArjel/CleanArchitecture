using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagement.Domain.CacheRepositories;
using EmployeeManagement.Domain.Dtos.DepartmentDtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using EmployeeManagement.Infrastructure.Data.CacheKeys;
using Microsoft.Extensions.Caching.Distributed;
using TanvirArjel.Extensions.Microsoft.Caching;

namespace EmployeeManagement.Infrastructure.Data.CacheRepositories
{
    internal class DepartmentCacheRepository : IDepartmentCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentCacheRepository(IDistributedCache distributedCache, IDepartmentRepository departmentRepository)
        {
            _distributedCache = distributedCache;
            _departmentRepository = departmentRepository;
        }

        public async Task<List<DepartmentDetailsDto>> GetListAsync()
        {
            string cacheKey = DepartmentCacheKeys.ListKey;
            List<DepartmentDetailsDto> departmentList = await _distributedCache.GetAsync<List<DepartmentDetailsDto>>(cacheKey);

            if (departmentList == null)
            {
                departmentList = await _departmentRepository.GetListAsync();

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
                departmentSelectList = await _departmentRepository.GetSelectListAsync();

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
                department = await _departmentRepository.GetByIdAsync(departmentId);

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
                department = await _departmentRepository.GetDetailsByIdAsync(departmentId);

                await _distributedCache.SetAsync<DepartmentDetailsDto>(cacheKey, department);
            }

            return department;
        }
    }
}
