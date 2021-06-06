using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using EmployeeManagement.Application.Exceptions;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.CacheRepositories;
using EmployeeManagement.Domain.Dtos.DepartmentDtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Implementations.Services
{
    public class DeparmentService : IDepartmentService
    {
        private readonly IDepartmentCacheRepository _departmentCacheRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public DeparmentService(IDepartmentCacheRepository departmentCacheRepository, IDepartmentRepository departmentRepository)
        {
            _departmentCacheRepository = departmentCacheRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<List<DepartmentDetailsDto>> GetDepartmentListAsync()
        {
            List<DepartmentDetailsDto> departmentList = await _departmentCacheRepository.GetListAsync();
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
                Name = createDepartmentDto.DepartmentName,
                Description = createDepartmentDto.Description
            };

            int departmentId = await _departmentRepository.InsertAsync(departmentToBeCreated);

            return departmentId;
        }

        public async Task<SelectList> GetDepartmentSelectListAsync(int? selectedDepartmentId)
        {
            List<DepartmentSelectListDto> departments = await _departmentCacheRepository.GetSelectListAsync();
            return new SelectList(departments, "DepartmentId", "DepartmentName", selectedDepartmentId);
        }

        public async Task<DepartmentDetailsDto> GetDepartmentAsync(int departmentId)
        {
            DepartmentDetailsDto departmentDetailsDto = await _departmentCacheRepository.GetDetailsByIdAsync(departmentId);

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

            departmentToBeUpdated.Name = updateDepartmentDto.DepartmentName;
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
            bool isExists = await _departmentRepository.Departments.AnyAsync(d => d.Id == departmentId);
            return isExists;
        }
    }
}
