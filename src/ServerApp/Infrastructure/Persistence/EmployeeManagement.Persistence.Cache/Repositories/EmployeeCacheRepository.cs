using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EmployeeManagement.Application.Caching.Repositories;
using EmployeeManagement.Application.Queries.EmployeeQueries;
using EmployeeManagement.Domain.Aggregates.EmployeeAggregate;
using EmployeeManagement.Persistence.Cache.Keys;
using Microsoft.Extensions.Caching.Distributed;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.Caching;

namespace EmployeeManagement.Persistence.Cache.Repositories;

internal class EmployeeCacheRepository : IEmployeeCacheRepository
{
    private readonly IDistributedCache _distributedCache;
    private readonly IQueryRepository _repository;

    public EmployeeCacheRepository(IDistributedCache distributedCache, IQueryRepository repository)
    {
        _distributedCache = distributedCache;
        _repository = repository;
    }

    public async Task<EmployeeDetailsDto> GetDetailsByIdAsync(Guid employeeId)
    {
        string cacheKey = EmployeeCacheKeys.GetDetailsKey(employeeId);
        EmployeeDetailsDto employeeDetails = await _distributedCache.GetAsync<EmployeeDetailsDto>(cacheKey);

        if (employeeDetails == null)
        {
            Expression<Func<Employee, EmployeeDetailsDto>> selectExp = e => new EmployeeDetailsDto
            {
                Id = e.Id,
                Name = e.Name,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name,
                DateOfBirth = e.DateOfBirth,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                IsActive = e.IsActive,
                CreatedAtUtc = e.CreatedAtUtc,
                LastModifiedAtUtc = e.LastModifiedAtUtc
            };

            employeeDetails = await _repository.GetByIdAsync(employeeId, selectExp);

            await _distributedCache.SetAsync(cacheKey, employeeDetails);
        }

        return employeeDetails;
    }
}
