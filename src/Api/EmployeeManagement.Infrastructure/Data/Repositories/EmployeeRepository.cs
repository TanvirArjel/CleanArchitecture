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

namespace EmployeeManagement.Infrastructure.Data.Repositories
{
    internal class EmployeeRepository : IEmployeeRepository
    {
        private readonly IRepository<Employee> _repository;

        public EmployeeRepository(IRepository<Employee> repository)
        {
            _repository = repository;
        }

        public IQueryable<Employee> Employees => _repository.Entities;

        public async Task<PaginatedList<EmployeeDetailsDto>> GetListAsync(int pageNumber, int pageSize)
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

            PaginatedList<EmployeeDetailsDto> paginatedList = await Employees
                .Select(selectExpression).ToPaginatedListAsync<EmployeeDetailsDto>(pageNumber, pageSize);

            return paginatedList;
        }

        public async Task<Employee> GetByIdAsync(long employeeId)
        {
            return await Employees.Where(c => c.EmployeeId == employeeId).FirstOrDefaultAsync();
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
