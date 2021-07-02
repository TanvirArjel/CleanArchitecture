using System;
using System.Threading.Tasks;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace Identity.Application.Services
{
    [ScopedService]
    public interface IApplicationUserService
    {
        Task<bool> IsOldPasswordAsync(ApplicationUser applicationUser, string password);

        Task StorePasswordAsync(ApplicationUser applicationUser, string password);

        Task SendEmailVerificationCodeAsync(string email);

        Task<bool> HasActiveEmailConfirmationCodeAsync(string email);

        Task<IdentityError> VerifyEmailAsync(string email, string code);

        Task SendPasswordResetCodeAsync(string email);

        Task<IdentityError> ResetPasswordAsync(string email, string code, string newPassword);

        Task<RefreshToken> GetRefreshTokenAsync(Guid userId);

        Task<bool> IsRefreshTokenValidAsync(Guid userId, string refreshToken);

        Task<RefreshToken> StoreRefreshTokenAsync(Guid userId, string token);

        Task<RefreshToken> UpdateRefreshTokenAsync(Guid userId, string token);

        Task UpdateDialCodeAsync(Guid userId, string dialCode);

        Task<string> GetLanguageCultureAsync(Guid userId);

        Task UpdateLanguageCultureAsync(Guid userId, string languageCulture);
    }
}
