using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Application.Exceptions;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Dtos.EmployeeDtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Implementations.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<List<EmployeeDetailsDto>> GetEmployeeListAsync()
        {
            Expression<Func<Employee, EmployeeDetailsDto>> selectExpression = e => new EmployeeDetailsDto
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
            };

            List<EmployeeDetailsDto> employeeDetailsDtos = await _employeeRepository.Employees.Select(selectExpression).ToListAsync();

            return employeeDetailsDtos;
        }

        public async Task<EmployeeDetailsDto> GetEmployeeDetailsAsync(int employeeId)
        {
            Expression<Func<Employee, EmployeeDetailsDto>> selectExpression = e => new EmployeeDetailsDto
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
            };

            EmployeeDetailsDto employeeDetailsDto = await _employeeRepository.Employees.Where(e => e.EmployeeId == employeeId)
                .Select(selectExpression).FirstOrDefaultAsync();

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
                if (employeeeToBeUpdated != null)
                {
                    employeeeToBeUpdated.EmployeeName = updateEmployeeDto.EmployeeName;
                    employeeeToBeUpdated.DepartmentId = updateEmployeeDto.DepartmentId;
                    employeeeToBeUpdated.DateOfBirth = updateEmployeeDto.DateOfBirth;
                    employeeeToBeUpdated.Email = updateEmployeeDto.Email;
                    employeeeToBeUpdated.PhoneNumber = updateEmployeeDto.PhoneNumber;

                    await _employeeRepository.UpdateAsync(employeeeToBeUpdated);
                }
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
