using System;
using System.Threading.Tasks;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace Identity.Application.Services
{
    [ScopedService]
    public interface IApplicationUserService
    {
        Task UpdateDialCodeAsync(Guid userId, string dialCode);

        Task<string> GetLanguageCultureAsync(Guid userId);

        Task UpdateLanguageCultureAsync(Guid userId, string languageCulture);
    }
}
