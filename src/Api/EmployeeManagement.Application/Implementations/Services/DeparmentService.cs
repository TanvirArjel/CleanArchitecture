using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using EmployeeManagement.Application.Exceptions;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Application.Implementations.Services
{
    public class DeparmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeparmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DepartmentDetailsDto>> GetDepartmentListAsync()
        {
            List<DepartmentDetailsDto> departmentList = await _unitOfWork.Repository<Department>().Entities
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

            await _unitOfWork.Repository<Department>().InsertEntityAsync(departmentToBeCreated);
            await _unitOfWork.SaveChangesAsync();

            return 1;
        }

        public async Task<SelectList> GetDepartmentSelectListAsync(int? selectedDepartmentId)
        {
            var departments = await _unitOfWork.Repository<Department>().Entities.Select(d => new
            {
                d.DepartmentId,
                d.DepartmentName
            }).ToListAsync();
            return new SelectList(departments, "DepartmentId", "DepartmentName", selectedDepartmentId);
        }

        public async Task<DepartmentDetailsDto> GetDepartmentAsync(int departmentId)
        {
            DepartmentDetailsDto departmentDetailsDto = await _unitOfWork.Repository<Department>().Entities.Where(d => d.DepartmentId == departmentId)
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

            Department departmentToBeUpdated = await _unitOfWork.Repository<Department>().GetEntityByIdAsync(updateDepartmentDto.DepartmentId);

            if (departmentToBeUpdated == null)
            {
                throw new EntityNotFoundException(typeof(Department), updateDepartmentDto.DepartmentId);
            }

            departmentToBeUpdated.DepartmentName = updateDepartmentDto.DepartmentName;
            departmentToBeUpdated.Description = updateDepartmentDto.Description;
            departmentToBeUpdated.IsActive = updateDepartmentDto.IsActive;
            _unitOfWork.Repository<Department>().UpdateEntity(departmentToBeUpdated);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteDepartment(int employeeId)
        {
            Department departmentToBeDeleted = await _unitOfWork.Repository<Department>().GetEntityByIdAsync(employeeId);
            if (departmentToBeDeleted != null)
            {
                _unitOfWork.Repository<Department>().DeleteEntity(departmentToBeDeleted);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> DepartmentExistsAsync(int departmentId)
        {
            bool isExists = await _unitOfWork.Repository<Department>().IsEntityExistsAsync(d => d.DepartmentId == departmentId);
            return isExists;
        }
    }
}
