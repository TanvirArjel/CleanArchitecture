using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository.Services;

namespace EmployeeManagement.Application.ServiceImplementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

            List<EmployeeDetailsDto> employeeDetailsDtos = await _unitOfWork.Repository<Employee>()
                .GetProjectedEntityListAsync(selectExpression);

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

            EmployeeDetailsDto employeeDetailsDto = await _unitOfWork.Repository<Employee>()
                .GetProjectedEntityByIdAsync(employeeId, selectExpression);

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
            await _unitOfWork.Repository<Employee>().InsertEntityAsync(employeeToBeCreated);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateEmplyeeAsync(UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                if (updateEmployeeDto == null)
                {
                    throw new ArgumentNullException(nameof(updateEmployeeDto));
                }

                Employee employeeeToBeUpdated = await _unitOfWork.Repository<Employee>().GetEntityByIdAsync(updateEmployeeDto.EmployeeId);
                if (employeeeToBeUpdated != null)
                {
                    employeeeToBeUpdated.EmployeeName = updateEmployeeDto.EmployeeName;
                    employeeeToBeUpdated.DepartmentId = updateEmployeeDto.DepartmentId;
                    employeeeToBeUpdated.DateOfBirth = updateEmployeeDto.DateOfBirth;
                    employeeeToBeUpdated.Email = updateEmployeeDto.Email;
                    employeeeToBeUpdated.PhoneNumber = updateEmployeeDto.PhoneNumber;

                    _unitOfWork.Repository<Employee>().UpdateEntity(employeeeToBeUpdated);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteEmployee(int employeeId)
        {
            Employee employeeeToBeDeleted = await _unitOfWork.Repository<Employee>().GetEntityByIdAsync(employeeId);
            if (employeeeToBeDeleted != null)
            {
                _unitOfWork.Repository<Employee>().DeleteEntity(employeeeToBeDeleted);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
