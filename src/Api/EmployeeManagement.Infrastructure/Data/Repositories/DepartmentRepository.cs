using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Data.Repositories
{
    internal class DepartmentRepository : IDepartmentRepository
    {
        private readonly IRepository<Department> _repository;

        public DepartmentRepository(IRepository<Department> repository)
        {
            _repository = repository;
        }

        public IQueryable<Department> Departments => _repository.Entities;

        public async Task<Department> GetByIdAsync(long commentId)
        {
            return await Departments.Where(c => c.DepartmentId == commentId).FirstOrDefaultAsync();
        }

        public async Task<int> InsertAsync(Department comment)
        {
            object[] primaryKeyValues = await _repository.InsertAsync(comment);
            return (int)primaryKeyValues[0];
        }

        public async Task UpdateAsync(Department comment)
        {
            await _repository.UpdateAsync(comment);
        }

        public async Task DeleteAsync(Department comment)
        {
            await _repository.DeleteAsync(comment);
        }
    }
}
