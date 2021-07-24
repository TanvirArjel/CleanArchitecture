using System.Threading.Tasks;
using Identity.Domain.Entities;

namespace Identity.Application.Services
{
    public interface IPasswordResetCodeService
    {
        Task<PasswordResetCode> GetAsync(string email, string code);
    }
}
