using System.Threading.Tasks;
using Identity.Domain.Entities;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace Identity.Application.Services
{
    [ScopedService]
    public interface IEmailVerificationCodeService
    {
        Task<EmailVerificationCode> GetAsync(string email, string code);

        Task<bool> HasActiveCodeAsync(string email);
    }
}
