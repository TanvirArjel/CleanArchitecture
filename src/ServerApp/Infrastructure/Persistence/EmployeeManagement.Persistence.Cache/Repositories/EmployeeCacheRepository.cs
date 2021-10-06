using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EmployeeManagement.Application.Caching.Repositories;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Domain.Aggregates.EmployeeAggregate;
using EmployeeManagement.Persistence.Cache.Keys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.Caching;

namespace EmployeeManagement.Persistence.Cache.Repositories
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

        public async Task<Employee> GetByIdAsync(Guid employeeId)
        {
            string cacheKey = EmployeeCacheKeys.GetKey(employeeId);
            Employee employee = await _distributedCache.GetAsync<Employee>(cacheKey);

            if (employee == null)
            {
                employee = await _repository.GetByIdAsync<Employee>(employeeId);

                await _distributedCache.SetAsync(cacheKey, employee);
            }

            return employee;
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

        public async Task UpdateAsync(Employee employee)
        {
            await _repository.UpdateAsync(employee);

            // Update item in cache
            Employee updatedEmployee = await _repository.GetByIdAsync<Employee>(employee.Id, q => q.Include(emp => emp.Department));

            string departmentCacheKey = EmployeeCacheKeys.GetKey(employee.Id);
            await _distributedCache.SetAsync(departmentCacheKey, updatedEmployee);

            string departmentDetailsCacheKey = EmployeeCacheKeys.GetDetailsKey(employee.Id);

            EmployeeDetailsDto departmentDetailsDto = new EmployeeDetailsDto()
            {
                Id = updatedEmployee.Id,
                Name = updatedEmployee.Name,
                DepartmentId = updatedEmployee.DepartmentId,
                DepartmentName = updatedEmployee.Department.Name,
                Email = updatedEmployee.Email,
                DateOfBirth = updatedEmployee.DateOfBirth,
                PhoneNumber = updatedEmployee.PhoneNumber,
                IsActive = updatedEmployee.IsActive,
                CreatedAtUtc = updatedEmployee.CreatedAtUtc,
                LastModifiedAtUtc = updatedEmployee.LastModifiedAtUtc
            };

            await _distributedCache.SetAsync(departmentDetailsCacheKey, departmentDetailsDto);
        }
    }
}
