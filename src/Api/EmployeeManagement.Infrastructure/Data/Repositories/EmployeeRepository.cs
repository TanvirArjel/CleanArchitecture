using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
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
