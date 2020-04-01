using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Services.EmployeeService
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
            List<EmployeeDetailsDto> employeeDetailsDtos = await _unitOfWork.Repository<Employee>().Entities
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
                }).ToListAsync();

            return employeeDetailsDtos;
        }

        public async Task<EmployeeDetailsDto> GetEmployeeDetailsAsync(int employeeId)
        {
            EmployeeDetailsDto employeeDetailsDto = await _unitOfWork.Repository<Employee>().Entities
                .Where(e => e.EmployeeId == employeeId).Select(e => new EmployeeDetailsDto
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
