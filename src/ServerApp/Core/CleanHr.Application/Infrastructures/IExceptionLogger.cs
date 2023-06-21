using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Application.Infrastructures;

[SingletonService]
public interface IExceptionLogger
{
    Task LogAsync(Exception exception);

    Task LogAsync(Exception exception, object parameters);

    Task LogAsync(Exception exception, string requestPath, string requestBody);
}
