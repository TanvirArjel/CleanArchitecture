using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Identity.Application.Dtos.ApplicationUserDtos;
using Identity.Application.Infrastrucures;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Application.Implementations.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly IRepository _repository;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IViewRenderService _viewRenderService;

        public ApplicationUserService(
            IEmailSender emailSender,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IViewRenderService viewRenderService,
            IRepository repository)
        {
            _emailSender = emailSender;
            _passwordHasher = passwordHasher;
            _viewRenderService = viewRenderService;
            _repository = repository;
        }

        public async Task<bool> IsOldPasswordAsync(ApplicationUser applicationUser, string password)
        {
            if (applicationUser == null)
            {
                throw new ArgumentNullException(nameof(applicationUser));
            }

            List<UserOldPasswordDto> userOldPasswords = await _repository.GetQueryable<UserOldPassword>()
               .Where(uop => uop.UserId == applicationUser.Id)
               .Select(uop => new UserOldPasswordDto
               {
                   UserId = uop.UserId,
                   Password = uop.PasswordHash,
                   SetAtUtc = uop.SetAtUtc
               }).ToListAsync();

            if (userOldPasswords.Any())
            {
                foreach (UserOldPasswordDto item in userOldPasswords)
                {
                    PasswordVerificationResult passwordVerificationResult = _passwordHasher.VerifyHashedPassword(applicationUser, item.Password, password);
                    if (passwordVerificationResult == PasswordVerificationResult.Success)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task StorePasswordAsync(ApplicationUser applicationUser, string password)
        {
            if (applicationUser == null)
            {
                throw new ArgumentNullException(nameof(applicationUser));
            }

            string passwordHash = _passwordHasher.HashPassword(applicationUser, password);

            UserOldPassword userOldPassword = new UserOldPassword()
            {
                UserId = applicationUser.Id,
                PasswordHash = passwordHash,
                SetAtUtc = DateTime.UtcNow
            };

            await _repository.InsertAsync(userOldPassword);
        }

        public async Task SendEmailVerificationCodeAsync(string email)
        {
            Random generator = new Random();
            string verificationCode = generator.Next(0, 1000000).ToString("D6", CultureInfo.InvariantCulture);

            EmailVerificationCode emailVerificationCode = new EmailVerificationCode()
            {
                Code = verificationCode,
                Email = email,
                SentAtUtc = DateTime.UtcNow
            };

            await _repository.InsertAsync(emailVerificationCode);

            (string Email, string VerificationCode) model = (email, verificationCode);
            string emailBody = await _viewRenderService.RenderViewToStringAsync("EmailTemplates/ConfirmRegistrationCodeTemplate", model);

            string senderEmail = "noreply@yourapp.com";
            string subject = "User Registration";

            EmailObject emailObject = new EmailObject(email, email, senderEmail, senderEmail, subject, emailBody);

            await _emailSender.SendAsync(emailObject);
        }

        public async Task<bool> HasActiveEmailConfirmationCodeAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            bool isExists = await _repository.ExistsAsync<EmailVerificationCode>(evc => evc.SentAtUtc.AddMinutes(5) > DateTime.UtcNow);
            return isExists;
        }

        public async Task<IdentityError> VerifyEmailAsync(string email, string code)
        {
            IDbContextTransaction dbContextTransaction = await _repository.BeginTransactionAsync();

            try
            {
                IdentityError identityError = null;

                EmailVerificationCode emailVerificationCode = await _repository
                .GetAsync<EmailVerificationCode>(evc => evc.Email == email && evc.Code == code && evc.UsedAtUtc == null);

                if (emailVerificationCode == null)
                {
                    identityError = new IdentityError()
                    {
                        Code = "Code",
                        Description = "Either email or password reset code is incorrect."
                    };

                    return identityError;
                }

                if (DateTime.UtcNow > emailVerificationCode.SentAtUtc.AddMinutes(5))
                {
                    identityError = new IdentityError()
                    {
                        Code = "Code",
                        Description = "The code is expired."
                    };

                    return identityError;
                }

                ApplicationUser applicationUser = await _repository.GetAsync<ApplicationUser>(au => au.Email == email);

                if (applicationUser == null)
                {
                    identityError = new IdentityError()
                    {
                        Code = "Email",
                        Description = "The provided email is not related to any account."
                    };
                }
                else
                {
                    applicationUser.EmailConfirmed = true;
                    await _repository.UpdateAsync(applicationUser);

                    emailVerificationCode.UsedAtUtc = DateTime.UtcNow;
                    await _repository.UpdateAsync(emailVerificationCode);

                    await dbContextTransaction.CommitAsync();
                }

                return identityError;
            }
            catch (Exception)
            {
                await dbContextTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task SendPasswordResetCodeAsync(string email)
        {
            Random generator = new Random();
            string verificationCode = generator.Next(0, 1000000).ToString("D6", CultureInfo.InvariantCulture);

            PasswordResetCode emailVerificationCode = new PasswordResetCode()
            {
                Code = verificationCode,
                Email = email,
                SentAtUtc = DateTime.UtcNow
            };

            await _repository.InsertAsync(emailVerificationCode);

            (string Email, string VerificationCode) model = (email, verificationCode);
            string subject = "Reset Password";
            string senderEmail = "noreply@yourapp.com";
            string emailBody = await _viewRenderService.RenderViewToStringAsync("EmailTemplates/PasswordResetCodeTemplate", model);
            EmailObject emailObject = new EmailObject(email, email, senderEmail, senderEmail, subject, emailBody);
            await _emailSender.SendAsync(emailObject);
        }

        public async Task<IdentityError> ResetPasswordAsync(string email, string code, string newPassword)
        {
            IDbContextTransaction dbContextTransaction = await _repository.BeginTransactionAsync();

            try
            {
                IdentityError identityError = null;

                PasswordResetCode emailVerificationCode = await _repository
                .GetAsync<PasswordResetCode>(evc => evc.Email == email && evc.Code == code && evc.UsedAtUtc == null);

                if (emailVerificationCode == null)
                {
                    identityError = new IdentityError()
                    {
                        Code = "Code",
                        Description = "Either email or password reset code is incorrect."
                    };

                    return identityError;
                }

                if (DateTime.UtcNow > emailVerificationCode.SentAtUtc.AddMinutes(5))
                {
                    identityError = new IdentityError()
                    {
                        Code = "Code",
                        Description = "The code is expired."
                    };
                }
                else
                {
                    ApplicationUser applicationUser = await _repository.GetAsync<ApplicationUser>(au => au.Email == email);

                    if (applicationUser == null)
                    {
                        identityError = new IdentityError()
                        {
                            Code = "Email",
                            Description = "The provided email is not related to any account."
                        };
                    }
                    else
                    {
                        string newHashedPassword = _passwordHasher.HashPassword(applicationUser, newPassword);
                        applicationUser.PasswordHash = newHashedPassword;
                        await _repository.UpdateAsync(applicationUser);

                        emailVerificationCode.UsedAtUtc = DateTime.UtcNow;
                        await _repository.UpdateAsync(emailVerificationCode);

                        await dbContextTransaction.CommitAsync();
                    }
                }

                return identityError;
            }
            catch (Exception)
            {
                await dbContextTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(Guid userId)
        {
            userId.ThrowIfEmpty(nameof(userId));

            RefreshToken refreshToken = await _repository.GetAsync<RefreshToken>(rt => rt.UserId == userId);

            return refreshToken;
        }

        public async Task<bool> IsRefreshTokenValidAsync(Guid userId, string refreshToken)
        {
            userId.ThrowIfEmpty(nameof(userId));

            if (refreshToken == null)
            {
                throw new ArgumentNullException(nameof(refreshToken));
            }

            bool isRefreshTokenValid = await _repository.ExistsAsync<RefreshToken>(rt => rt.UserId == userId && rt.Token == refreshToken);

            return isRefreshTokenValid;
        }

        public async Task<RefreshToken> StoreRefreshTokenAsync(Guid userId, string token)
        {
            userId.ThrowIfEmpty(nameof(userId));

            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            RefreshToken refreshToken = new RefreshToken()
            {
                UserId = userId,
                Token = token,
                CreatedAtUtc = DateTime.UtcNow,
                ExpireAtUtc = DateTime.UtcNow.AddDays(30)
            };

            await _repository.InsertAsync(refreshToken);
            return refreshToken;
        }

        public async Task<RefreshToken> UpdateRefreshTokenAsync(Guid userId, string token)
        {
            userId.ThrowIfEmpty(nameof(userId));

            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            RefreshToken refreshTokenToBeUpdated = await _repository.GetAsync<RefreshToken>(rt => rt.UserId == userId);

            if (refreshTokenToBeUpdated == null)
            {
                throw new InvalidOperationException($"The RefreshToken does not exist with id value: {userId}.");
            }

            refreshTokenToBeUpdated.Token = token;
            refreshTokenToBeUpdated.ExpireAtUtc = DateTime.UtcNow;
            refreshTokenToBeUpdated.CreatedAtUtc = DateTime.UtcNow.AddDays(10);

            await _repository.UpdateAsync(refreshTokenToBeUpdated);

            return refreshTokenToBeUpdated;
        }

        public async Task UpdateDialCodeAsync(Guid userId, string dialCode)
        {
            userId.ThrowIfEmpty(nameof(userId));

            if (dialCode == null)
            {
                throw new ArgumentNullException(nameof(dialCode));
            }

            ApplicationUser applicationUserToBeUpdated = await _repository.GetByIdAsync<ApplicationUser>(userId);

            if (applicationUserToBeUpdated == null)
            {
                throw new InvalidOperationException($"The ApplicationUser does not exist with id value: {userId}.");
            }

            applicationUserToBeUpdated.DialCode = dialCode.StartsWith('+') ? dialCode : $"+{dialCode}";
            await _repository.UpdateAsync(applicationUserToBeUpdated);
        }

        public async Task<string> GetLanguageCultureAsync(Guid userId)
        {
            userId.ThrowIfEmpty(nameof(userId));

            string userLanguageCulture = await _repository.GetQueryable<ApplicationUser>().Where(u => u.Id == userId)
                .Select(u => u.LanguageCulture).FirstOrDefaultAsync();

            return userLanguageCulture;
        }

        public async Task UpdateLanguageCultureAsync(Guid userId, string languageCulture)
        {
            userId.ThrowIfEmpty(nameof(userId));

            if (string.IsNullOrWhiteSpace(languageCulture))
            {
                throw new ArgumentNullException(nameof(languageCulture));
            }

            ApplicationUser userToBeUpdated = await _repository.GetByIdAsync<ApplicationUser>(userId);

            if (userToBeUpdated == null)
            {
                throw new InvalidOperationException($"The ApplicationUser does not exist with id value: {userId}.");
            }

            userToBeUpdated.LanguageCulture = languageCulture;
            await _repository.UpdateAsync(userToBeUpdated);
        }
    }
}
