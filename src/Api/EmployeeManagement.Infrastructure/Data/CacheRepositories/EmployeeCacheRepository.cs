using System.Threading.Tasks;
using EmployeeManagement.Domain.CacheRepositories;
using EmployeeManagement.Domain.Dtos.EmployeeDtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using EmployeeManagement.Infrastructure.Data.CacheKeys;
using Microsoft.Extensions.Caching.Distributed;
using TanvirArjel.Extensions.Microsoft.Caching;

namespace EmployeeManagement.Infrastructure.Data.CacheRepositories
{
    internal class EmployeeCacheRepository : IEmployeeCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeCacheRepository(IDistributedCache distributedCache, IEmployeeRepository employeeRepository)
        {
            _distributedCache = distributedCache;
            _employeeRepository = employeeRepository;
        }

        public async Task<Employee> GetByIdAsync(long employeeId)
        {
            string cacheKey = EmployeeCacheKeys.GetKey(employeeId);
            Employee employee = await _distributedCache.GetAsync<Employee>(cacheKey);

            if (employee == null)
            {
                employee = await _employeeRepository.GetByIdAsync(employeeId);

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
                employeeDetails = await _employeeRepository.GetDetailsByIdAsync(employeeId);

                await _distributedCache.SetAsync<EmployeeDetailsDto>(cacheKey, employeeDetails);
            }

            return employeeDetails;
        }
    }
}
