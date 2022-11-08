using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Application.Caching.Handlers;

[ScopedService]
public interface IDepartmentCacheHandler
{
    Task RemoveListAsync();
}
