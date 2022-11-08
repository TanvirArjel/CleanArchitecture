using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CleanHr.Application.Caching.Repositories;
using CleanHr.Application.Queries.DepartmentQueries;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Persistence.Cache.Keys;
using Microsoft.Extensions.Caching.Distributed;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.Caching;

namespace CleanHr.Persistence.Cache.Repositories;

internal class DepartmentCacheRepository : IDepartmentCacheRepository
{
    private readonly IDistributedCache _distributedCache;
    private readonly IQueryRepository _repository;

    public DepartmentCacheRepository(IDistributedCache distributedCache, IQueryRepository repository)
    {
        _distributedCache = distributedCache;
        _repository = repository;
    }

    public async Task<List<DepartmentDto>> GetListAsync()
    {
        string cacheKey = DepartmentCacheKeys.ListKey;
        List<DepartmentDto> departmentList = await _distributedCache.GetAsync<List<DepartmentDto>>(cacheKey);

        if (departmentList == null)
        {
            Expression<Func<Department, DepartmentDto>> selectExp = d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name.Value,
                Description = d.Description,
                IsActive = d.IsActive,
                CreatedAtUtc = d.CreatedAtUtc,
                LastModifiedAtUtc = d.LastModifiedAtUtc
            };

            departmentList = await _repository.GetListAsync(selectExp);

            await _distributedCache.SetAsync(cacheKey, departmentList);
        }

        return departmentList;
    }

    public async Task<Department> GetByIdAsync(Guid departmentId)
    {
        string cacheKey = DepartmentCacheKeys.GetKey(departmentId);
        Department department = await _distributedCache.GetAsync<Department>(cacheKey);

        if (department == null)
        {
            department = await _repository.GetByIdAsync<Department>(departmentId);

            await _distributedCache.SetAsync(cacheKey, department);
        }

        return department;
    }

    public async Task<DepartmentDetailsDto> GetDetailsByIdAsync(Guid departmentId)
    {
        string cacheKey = DepartmentCacheKeys.GetDetailsKey(departmentId);
        DepartmentDetailsDto department = await _distributedCache.GetAsync<DepartmentDetailsDto>(cacheKey);

        if (department == null)
        {
            Expression<Func<Department, DepartmentDetailsDto>> selectExp = d => new DepartmentDetailsDto
            {
                Id = d.Id,
                Name = d.Name.Value,
                Description = d.Description,
                IsActive = d.IsActive,
                CreatedAtUtc = d.CreatedAtUtc,
                LastModifiedAtUtc = d.LastModifiedAtUtc
            };

            department = await _repository.GetByIdAsync(departmentId, selectExp);

            await _distributedCache.SetAsync(cacheKey, department);
        }

        return department;
    }
}
