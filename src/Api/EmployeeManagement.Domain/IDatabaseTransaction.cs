using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;
using Microsoft.EntityFrameworkCore.Storage;

namespace EmployeeManagement.Domain
{
    public interface IDatabaseTransaction : IScopedService
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
