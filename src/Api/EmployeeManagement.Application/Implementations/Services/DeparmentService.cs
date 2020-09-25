using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using EmployeeManagement.Application.Exceptions;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Implementations.Services
{
    public class DeparmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DeparmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<List<DepartmentDetailsDto>> GetDepartmentListAsync()
        {
            List<DepartmentDetailsDto> departmentList = await _departmentRepository.Departments
                .Select(d => new DepartmentDetailsDto
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    Description = d.Description,
                    IsActive = d.IsActive,
                    CreatedAtUtc = d.CreatedAtUtc,
                    LastModifiedAtUtc = d.LastModifiedAtUtc
                }).ToListAsync();

            return departmentList;
        }

        public async Task<int> CreateDepartmentAsync(CreateDepartmentDto createDepartmentDto)
        {
            if (createDepartmentDto == null)
            {
                throw new ArgumentNullException(nameof(createDepartmentDto));
            }

            Department departmentToBeCreated = new Department()
            {
                DepartmentName = createDepartmentDto.DepartmentName,
                Description = createDepartmentDto.Description
            };

            int departmentId = await _departmentRepository.InsertAsync(departmentToBeCreated);

            return departmentId;
        }

        public async Task<SelectList> GetDepartmentSelectListAsync(int? selectedDepartmentId)
        {
            var departments = await _departmentRepository.Departments.Select(d => new
            {
                d.DepartmentId,
                d.DepartmentName
            }).ToListAsync();
            return new SelectList(departments, "DepartmentId", "DepartmentName", selectedDepartmentId);
        }

        public async Task<DepartmentDetailsDto> GetDepartmentAsync(int departmentId)
        {
            DepartmentDetailsDto departmentDetailsDto = await _departmentRepository.Departments.Where(d => d.DepartmentId == departmentId)
                .Select(d => new DepartmentDetailsDto
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    Description = d.Description,
                    IsActive = d.IsActive,
                    CreatedAtUtc = d.CreatedAtUtc,
                    LastModifiedAtUtc = d.LastModifiedAtUtc
                }).FirstOrDefaultAsync();

            return departmentDetailsDto;
        }

        public async Task UpdateDepartmentAsync(UpdateDepartmentDto updateDepartmentDto)
        {
            if (updateDepartmentDto == null)
            {
                throw new ArgumentNullException(nameof(updateDepartmentDto));
            }

            Department departmentToBeUpdated = await _departmentRepository.GetByIdAsync(updateDepartmentDto.DepartmentId);

            if (departmentToBeUpdated == null)
            {
                throw new EntityNotFoundException(typeof(Department), updateDepartmentDto.DepartmentId);
            }

            departmentToBeUpdated.DepartmentName = updateDepartmentDto.DepartmentName;
            departmentToBeUpdated.Description = updateDepartmentDto.Description;
            departmentToBeUpdated.IsActive = updateDepartmentDto.IsActive;
            await _departmentRepository.UpdateAsync(departmentToBeUpdated);
        }

        public async Task DeleteDepartment(int departmentId)
        {
            Department departmentToBeDeleted = await _departmentRepository.GetByIdAsync(departmentId);

            if (departmentToBeDeleted == null)
            {
                throw new EntityNotFoundException(typeof(Department), departmentId);
            }

            await _departmentRepository.DeleteAsync(departmentToBeDeleted);
        }

        public async Task<bool> DepartmentExistsAsync(int departmentId)
        {
            bool isExists = await _departmentRepository.Departments.AnyAsync(d => d.DepartmentId == departmentId);
            return isExists;
        }
    }
}
