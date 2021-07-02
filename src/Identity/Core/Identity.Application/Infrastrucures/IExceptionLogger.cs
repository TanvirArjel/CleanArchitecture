using System;
using System.Threading.Tasks;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace Identity.Application.Infrastrucures
{
    [SingletonService]
    public interface IExceptionLogger
    {
        Task LogAsync(Exception exception);

        Task LogAsync(Exception exception, object paramters);

        Task LogAsync(Exception exception, string requestPath, string requestBody);
    }
}
