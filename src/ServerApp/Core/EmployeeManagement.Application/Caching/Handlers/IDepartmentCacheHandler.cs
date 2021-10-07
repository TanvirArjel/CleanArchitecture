using System.Threading.Tasks;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Application.Caching.Handlers
{
    [ScopedService]
    public interface IDepartmentCacheHandler
    {
        Task RemoveListAsync();
    }
}
