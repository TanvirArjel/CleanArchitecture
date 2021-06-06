using System;
using System.Threading.Tasks;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Application.Infrastrucures
{
    [ScopedService]
    public interface IExceptionLogger
    {
        Task LogAsync(Exception exception);

        Task LogAsync(Exception exception, object paramters);
    }
}
