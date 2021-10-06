using System;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Caching.Handlers
{
    public interface IEmployeeCacheHandler
    {
        Task RemoveDetailsByIdAsync(Guid employeeId);
    }
}
