using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Domain
{
    [ScopedService]
    public interface IDatabaseTransaction
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
