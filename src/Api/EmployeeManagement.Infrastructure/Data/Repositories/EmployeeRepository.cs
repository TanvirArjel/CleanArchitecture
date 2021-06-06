using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Dtos.EmployeeDtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using EmployeeManagement.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Infrastructure.Data.Repositories
{
    internal class EmployeeRepository : IEmployeeRepository
    {
        private readonly IRepository _repository;

        public EmployeeRepository(IRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<Employee> Employees => _repository.GetQueryable<Employee>();

        public async Task<PaginatedList<EmployeeDetailsDto>> GetListAsync(int pageNumber, int pageSize)
        {
            Expression<Func<Employee, EmployeeDetailsDto>> selectExpression = e => new EmployeeDetailsDto
            {
                EmployeeId = e.Id,
                EmployeeName = e.Name,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name,
                DateOfBirth = e.DateOfBirth,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                IsActive = e.IsActive,
                CreatedAtUtc = e.CreatedAtUtc,
                LastModifiedAtUtc = e.LastModifiedAtUtc
            };

            PaginatedList<EmployeeDetailsDto> paginatedList = await Employees
                .Select(selectExpression).ToPaginatedListAsync<EmployeeDetailsDto>(pageNumber, pageSize);

            return paginatedList;
        }

        public async Task<Employee> GetByIdAsync(long employeeId)
        {
            return await Employees.Where(c => c.Id == employeeId).FirstOrDefaultAsync();
        }

        public async Task<EmployeeDetailsDto> GetDetailsByIdAsync(long employeeId)
        {
            EmployeeDetailsDto employeeDetailsDto = await Employees.Where(e => e.Id == employeeId)
                    .Select(e => new EmployeeDetailsDto
                    {
                        EmployeeId = e.Id,
                        EmployeeName = e.Name,
                        DepartmentId = e.DepartmentId,
                        DepartmentName = e.Department.Name,
                        DateOfBirth = e.DateOfBirth,
                        Email = e.Email,
                        PhoneNumber = e.PhoneNumber,
                        IsActive = e.IsActive,
                        CreatedAtUtc = e.CreatedAtUtc,
                        LastModifiedAtUtc = e.LastModifiedAtUtc
                    }).FirstOrDefaultAsync();

            return employeeDetailsDto;
        }

        public async Task InsertAsync(Employee employee)
        {
            await _repository.InsertAsync(employee);
        }

        public async Task UpdateAsync(Employee employee)
        {
            await _repository.UpdateAsync(employee);
        }

        public async Task DeleteAsync(Employee employee)
        {
            await _repository.DeleteAsync(employee);
        }
    }
}
