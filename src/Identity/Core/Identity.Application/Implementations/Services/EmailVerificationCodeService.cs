using System;
using System.Threading.Tasks;
using Identity.Application.Services;
using Identity.Domain.Entities;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace Identity.Application.Implementations.Services
{
    internal class EmailVerificationCodeService : IEmailVerificationCodeService
    {
        private readonly IRepository _repository;

        public EmailVerificationCodeService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<EmailVerificationCode> GetAsync(string email, string code)
        {
            email.ThrowIfNotValidEmail(nameof(email));
            code.ThrowIfNullOrEmpty(nameof(code));

            EmailVerificationCode emailVerificationCode = await _repository
                .GetAsync<EmailVerificationCode>(evc => evc.Email == email && evc.Code == code && evc.UsedAtUtc == null);

            return emailVerificationCode;
        }

        public async Task<bool> HasActiveCodeAsync(string email)
        {
            email.ThrowIfNotValidEmail(nameof(email));

            bool isExists = await _repository
                .ExistsAsync<EmailVerificationCode>(evc => evc.Email == email && evc.SentAtUtc.AddMinutes(5) > DateTime.UtcNow);
            return isExists;
        }
    }
}
