using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using EmployeeManagement.Domain.CacheRepositories;
using EmployeeManagement.Domain.Dtos.EmployeeDtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Data.CacheKeys;
using EmployeeManagement.Infrastructure.Data.Extensions;
using EmployeeManagement.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace EmployeeManagement.Infrastructure.Data.CacheRepositories
{
    internal class EmployeeCacheRepository : IEmployeeCacheRepository
    {
        private readonly IRepository<Employee> _repository;
        private readonly IDistributedCache _distributedCache;

        public EmployeeCacheRepository(IRepository<Employee> repository, IDistributedCache distributedCache)
        {
            _repository = repository;
            _distributedCache = distributedCache;
        }

        public async Task<Employee> GetByIdAsync(long employeeId)
        {
            string cacheKey = EmployeeCacheKeys.GetKey(employeeId);
            Employee employee = await _distributedCache.GetAsync<Employee>(cacheKey);

            if (employee == null)
            {
                employee = await _repository.Entities.AsNoTracking().FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

                await _distributedCache.SetAsync<Employee>(cacheKey, employee);
            }

            return employee;
        }

        public async Task<EmployeeDetailsDto> GetDetailsAsync(long employeeId)
        {
            string cacheKey = EmployeeCacheKeys.GetDetailsKey(employeeId);
            EmployeeDetailsDto employeeDetails = await _distributedCache.GetAsync<EmployeeDetailsDto>(cacheKey);

            if (employeeDetails == null)
            {
                employeeDetails = await _repository.Entities.Where(e => e.EmployeeId == employeeId)
                    .Select(e => new EmployeeDetailsDto
                    {
                        EmployeeId = e.EmployeeId,
                        EmployeeName = e.EmployeeName,
                        DepartmentId = e.DepartmentId,
                        DepartmentName = e.Department.DepartmentName,
                        DateOfBirth = e.DateOfBirth,
                        Email = e.Email,
                        PhoneNumber = e.PhoneNumber,
                        IsActive = e.IsActive,
                        CreatedAtUtc = e.CreatedAtUtc,
                        LastModifiedAtUtc = e.LastModifiedAtUtc
                    }).FirstOrDefaultAsync();

                await _distributedCache.SetAsync<EmployeeDetailsDto>(cacheKey, employeeDetails);
            }

            return employeeDetails;
        }
    }
}
