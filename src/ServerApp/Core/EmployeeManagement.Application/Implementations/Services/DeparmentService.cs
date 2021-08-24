using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagement.Application.CacheRepositories;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using EmployeeManagement.Application.Exceptions;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using Microsoft.AspNetCore.Mvc.Rendering;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Application.Implementations.Services
{
    public class DeparmentService : IDepartmentService
    {
        private readonly DepartmentDomainService _departmentDomainService;
        private readonly IDepartmentCacheRepository _departmentCacheRepository;
        private readonly IRepository _repository;

        public DeparmentService(
            IDepartmentCacheRepository departmentCacheRepository,
            IRepository repository,
            DepartmentDomainService departmentDomainService)
        {
            _departmentCacheRepository = departmentCacheRepository;
            _repository = repository;
            _departmentDomainService = departmentDomainService;
        }

        public async Task<List<DepartmentDetailsDto>> GetListAsync()
        {
            List<DepartmentDetailsDto> departmentList = await _departmentCacheRepository.GetListAsync();
            return departmentList;
        }

        public async Task<Guid> CreateAsync(CreateDepartmentDto createDepartmentDto)
        {
            createDepartmentDto.ThrowIfNull(nameof(createDepartmentDto));

            Department departmentToBeCreated = await _departmentDomainService.CreateAsync(createDepartmentDto.Name, createDepartmentDto.Description);

            Guid departmentId = await _departmentCacheRepository.InsertAsync(departmentToBeCreated);

            return departmentId;
        }

        public async Task<SelectList> GetSelectListAsync(Guid? selectedDepartmentId)
        {
            List<DepartmentSelectListDto> departments = await _departmentCacheRepository.GetSelectListAsync();
            return new SelectList(departments, "Id", "Name", selectedDepartmentId);
        }

        public async Task<DepartmentDetailsDto> GetByIdAsync(Guid departmentId)
        {
            departmentId.ThrowIfEmpty(nameof(departmentId));

            DepartmentDetailsDto departmentDetailsDto = await _departmentCacheRepository.GetDetailsByIdAsync(departmentId);

            return departmentDetailsDto;
        }

        public async Task UpdateAsync(UpdateDepartmentDto updateDepartmentDto)
        {
            updateDepartmentDto.ThrowIfNull(nameof(updateDepartmentDto));

            Department departmentToBeUpdated = await _repository.GetByIdAsync<Department>(updateDepartmentDto.Id);

            if (departmentToBeUpdated == null)
            {
                throw new EntityNotFoundException(typeof(Department), updateDepartmentDto.Id);
            }

            await _departmentDomainService.SetNameAsync(departmentToBeUpdated, updateDepartmentDto.Name);
            departmentToBeUpdated.SetDescription(updateDepartmentDto.Description);
            departmentToBeUpdated.IsActive = updateDepartmentDto.IsActive;
            await _departmentCacheRepository.UpdateAsync(departmentToBeUpdated);
        }

        public async Task DeleteAsync(Guid departmentId)
        {
            departmentId.ThrowIfEmpty(nameof(departmentId));

            Department departmentToBeDeleted = await _repository.GetByIdAsync<Department>(departmentId);

            if (departmentToBeDeleted == null)
            {
                throw new EntityNotFoundException(typeof(Department), departmentId);
            }

            await _departmentCacheRepository.DeleteAsync(departmentToBeDeleted);
        }

        public async Task<bool> ExistsAsync(Guid departmentId)
        {
            departmentId.ThrowIfEmpty(nameof(departmentId));

            bool isExists = await _repository.ExistsAsync<Department>(d => d.Id == departmentId);
            return isExists;
        }

        public async Task<bool> ExistsByNameAsync(string departmentName)
        {
            departmentName.ThrowIfNullOrEmpty(nameof(departmentName));

            bool isExistent = await _repository.ExistsAsync<Department>(d => d.Name == departmentName);
            return isExistent;
        }

        public async Task<bool> IsUniqueAsync(Guid departmentId, string departmentName)
        {
            departmentId.ThrowIfEmpty(nameof(departmentId));
            departmentName.ThrowIfNullOrEmpty(nameof(departmentName));

            bool isExistent = await _repository.ExistsAsync<Department>(d => d.Id != departmentId && d.Name == departmentName);
            return !isExistent;
        }
    }
}
