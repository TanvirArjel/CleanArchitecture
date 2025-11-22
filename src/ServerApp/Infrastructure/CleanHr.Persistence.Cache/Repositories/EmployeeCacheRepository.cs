using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CleanHr.Application.Caching.Repositories;
using CleanHr.Application.Queries.EmployeeQueries;
using CleanHr.Domain.Aggregates.EmployeeAggregate;
using CleanHr.Persistence.Cache.Keys;
using Microsoft.Extensions.Caching.Distributed;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.Caching;

namespace CleanHr.Persistence.Cache.Repositories;

internal sealed class EmployeeCacheRepository(IDistributedCache distributedCache, IQueryRepository repository) : IEmployeeCacheRepository
{
    public async Task<EmployeeDetailsDto> GetDetailsByIdAsync(Guid employeeId)
    {
        string cacheKey = EmployeeCacheKeys.GetDetailsKey(employeeId);
        EmployeeDetailsDto employeeDetails = await distributedCache.GetAsync<EmployeeDetailsDto>(cacheKey);

        if (employeeDetails == null)
        {
            Expression<Func<Employee, EmployeeDetailsDto>> selectExp = e => new EmployeeDetailsDto
            {
                Id = e.Id,
                Name = e.FirstName + " " + e.LastName,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name,
                DateOfBirth = e.DateOfBirth,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                IsActive = e.IsActive,
                CreatedAtUtc = e.CreatedAtUtc,
                LastModifiedAtUtc = e.LastModifiedAtUtc
            };

            employeeDetails = await repository.GetByIdAsync(employeeId, selectExp);

            await distributedCache.SetAsync(cacheKey, employeeDetails);
        }

        return employeeDetails;
    }
}
