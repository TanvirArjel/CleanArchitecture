using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EmployeeManagement.Application.CacheRepositories;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Data.CacheKeys;
using Microsoft.Extensions.Caching.Distributed;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.Caching;

namespace EmployeeManagement.Infrastructure.Data.CacheRepositories
{
    internal class EmployeeCacheRepository : IEmployeeCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepository _repository;

        public EmployeeCacheRepository(IDistributedCache distributedCache, IRepository repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public async Task<Employee> GetByIdAsync(long employeeId)
        {
            string cacheKey = EmployeeCacheKeys.GetKey(employeeId);
            Employee employee = await _distributedCache.GetAsync<Employee>(cacheKey);

            if (employee == null)
            {
                employee = await _repository.GetByIdAsync<Employee>(employeeId);

                await _distributedCache.SetAsync<Employee>(cacheKey, employee);
            }

            return employee;
        }

        public async Task<EmployeeDetailsDto> GetDetailsByIdAsync(long employeeId)
        {
            string cacheKey = EmployeeCacheKeys.GetDetailsKey(employeeId);
            EmployeeDetailsDto employeeDetails = await _distributedCache.GetAsync<EmployeeDetailsDto>(cacheKey);

            if (employeeDetails == null)
            {
                Expression<Func<Employee, EmployeeDetailsDto>> selectExp = e => new EmployeeDetailsDto
                {
                    EmployeeId = e.Id,
                    EmployeeName = e.Name,
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

                await _distributedCache.SetAsync<EmployeeDetailsDto>(cacheKey, employeeDetails);
            }

            return employeeDetails;
        }
    }
}
