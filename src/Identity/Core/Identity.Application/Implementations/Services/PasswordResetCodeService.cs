using System.Threading.Tasks;
using Identity.Application.Services;
using Identity.Domain.Entities;
using TanvirArjel.EFCore.GenericRepository;

namespace Identity.Application.Implementations.Services
{
    internal class PasswordResetCodeService : IPasswordResetCodeService
    {
        private readonly IRepository _repository;

        public PasswordResetCodeService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<PasswordResetCode> GetAsync(string email, string code)
        {
            PasswordResetCode passwordResetCode = await _repository
                .GetAsync<PasswordResetCode>(evc => evc.Email == email && evc.Code == code && evc.UsedAtUtc == null);

            return passwordResetCode;
        }
    }
}
