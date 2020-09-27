using System;
using System.Threading.Tasks;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Application.Exceptions;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.CacheRepositories;
using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Dtos.EmployeeDtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;

namespace EmployeeManagement.Application.Implementations.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeCacheRepository _employeeCacheRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, IEmployeeCacheRepository employeeCacheRepository)
        {
            _employeeRepository = employeeRepository;
            _employeeCacheRepository = employeeCacheRepository;
        }

        public async Task<PaginatedList<EmployeeDetailsDto>> GetEmployeeListAsync(int pageNumber, int pageSize)
        {
            PaginatedList<EmployeeDetailsDto> employeeDetailsDtos = await _employeeRepository.GetListAsync(pageNumber, pageSize);

            return employeeDetailsDtos;
        }

        public async Task<EmployeeDetailsDto> GetEmployeeDetailsAsync(int employeeId)
        {
            EmployeeDetailsDto employeeDetailsDto = await _employeeCacheRepository.GetDetailsAsync(employeeId);

            return employeeDetailsDto;
        }

        public async Task CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto)
        {
            if (createEmployeeDto == null)
            {
                throw new ArgumentNullException(nameof(createEmployeeDto));
            }

            Employee employeeToBeCreated = new Employee()
            {
                EmployeeName = createEmployeeDto.EmployeeName,
                DepartmentId = createEmployeeDto.DepartmentId,
                DateOfBirth = createEmployeeDto.DateOfBirth,
                Email = createEmployeeDto.Email,
                PhoneNumber = createEmployeeDto.PhoneNumber
            };

            await _employeeRepository.InsertAsync(employeeToBeCreated);
        }

        public async Task UpdateEmplyeeAsync(UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                if (updateEmployeeDto == null)
                {
                    throw new ArgumentNullException(nameof(updateEmployeeDto));
                }

                Employee employeeeToBeUpdated = await _employeeRepository.GetByIdAsync(updateEmployeeDto.EmployeeId);

                if (employeeeToBeUpdated == null)
                {
                    throw new EntityNotFoundException(typeof(Employee), updateEmployeeDto.EmployeeId);
                }

                employeeeToBeUpdated.EmployeeName = updateEmployeeDto.EmployeeName;
                employeeeToBeUpdated.DepartmentId = updateEmployeeDto.DepartmentId;
                employeeeToBeUpdated.DateOfBirth = updateEmployeeDto.DateOfBirth;
                employeeeToBeUpdated.Email = updateEmployeeDto.Email;
                employeeeToBeUpdated.PhoneNumber = updateEmployeeDto.PhoneNumber;

                await _employeeRepository.UpdateAsync(employeeeToBeUpdated);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteEmployee(int employeeId)
        {
            Employee employeeeToBeDeleted = await _employeeRepository.GetByIdAsync(employeeId);

            if (employeeeToBeDeleted == null)
            {
                throw new EntityNotFoundException(typeof(Employee), employeeId);
            }

            await _employeeRepository.DeleteAsync(employeeeToBeDeleted);
        }
    }
}
