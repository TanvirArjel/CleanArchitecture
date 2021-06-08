using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagement.Application.CacheRepositories;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using EmployeeManagement.Application.Exceptions;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Application.Implementations.Services
{
    public class DeparmentService : IDepartmentService
    {
        private readonly IDepartmentCacheRepository _departmentCacheRepository;
        private readonly IRepository _repository;

        public DeparmentService(IDepartmentCacheRepository departmentCacheRepository, IRepository repository)
        {
            _departmentCacheRepository = departmentCacheRepository;
            _repository = repository;
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

            int departmentId = await _departmentCacheRepository.InsertAsync(departmentToBeCreated);

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

            Department departmentToBeUpdated = await _repository.GetByIdAsync<Department>(updateDepartmentDto.DepartmentId);

            if (departmentToBeUpdated == null)
            {
                throw new EntityNotFoundException(typeof(Department), updateDepartmentDto.DepartmentId);
            }

            departmentToBeUpdated.Name = updateDepartmentDto.DepartmentName;
            departmentToBeUpdated.Description = updateDepartmentDto.Description;
            departmentToBeUpdated.IsActive = updateDepartmentDto.IsActive;
            await _departmentCacheRepository.UpdateAsync(departmentToBeUpdated);
        }

        public async Task DeleteDepartment(int departmentId)
        {
            Department departmentToBeDeleted = await _repository.GetByIdAsync<Department>(departmentId);

            if (departmentToBeDeleted == null)
            {
                throw new EntityNotFoundException(typeof(Department), departmentId);
            }

            await _departmentCacheRepository.DeleteAsync(departmentToBeDeleted);
        }

        public async Task<bool> DepartmentExistsAsync(int departmentId)
        {
            bool isExists = await _repository.ExistsAsync<Department>(d => d.Id == departmentId);
            return isExists;
        }
    }
}
