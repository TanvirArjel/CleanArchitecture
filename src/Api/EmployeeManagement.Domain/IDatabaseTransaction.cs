using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Domain
{
    public interface IDatabaseTransaction : IScopedService
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
