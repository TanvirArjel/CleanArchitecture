using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Application.Infrastrucures;

[SingletonService]
public interface IExceptionLogger
{
    Task LogAsync(Exception exception);

    Task LogAsync(Exception exception, object paramters);

    Task LogAsync(Exception exception, string requestPath, string requestBody);
}
