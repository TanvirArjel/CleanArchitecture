using System;
using System.Threading.Tasks;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace Identity.Application.Services
{
    [ScopedService]
    public interface IViewRenderService
    {
        Task<string> RenderViewToStringAsync(string viewNameOrPath, object model);

        Task<string> RenderViewToStringAsync(string viewNameOrPath, object model, Guid userId);
    }
}
