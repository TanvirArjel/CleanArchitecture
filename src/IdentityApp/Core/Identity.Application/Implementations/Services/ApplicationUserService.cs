using System;
using System.Linq;
using System.Threading.Tasks;
using Identity.Application.Infrastrucures;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Application.Implementations.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly IRepository _repository;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IViewRenderService _viewRenderService;

        public ApplicationUserService(
            IEmailSender emailSender,
            IPasswordHasher<User> passwordHasher,
            IViewRenderService viewRenderService,
            IRepository repository)
        {
            _emailSender = emailSender;
            _passwordHasher = passwordHasher;
            _viewRenderService = viewRenderService;
            _repository = repository;
        }

        public async Task UpdateDialCodeAsync(Guid userId, string dialCode)
        {
            userId.ThrowIfEmpty(nameof(userId));

            if (dialCode == null)
            {
                throw new ArgumentNullException(nameof(dialCode));
            }

            User applicationUserToBeUpdated = await _repository.GetByIdAsync<User>(userId);

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

            string userLanguageCulture = await _repository.GetQueryable<User>().Where(u => u.Id == userId)
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

            User userToBeUpdated = await _repository.GetByIdAsync<User>(userId);

            if (userToBeUpdated == null)
            {
                throw new InvalidOperationException($"The ApplicationUser does not exist with id value: {userId}.");
            }

            userToBeUpdated.LanguageCulture = languageCulture;
            await _repository.UpdateAsync(userToBeUpdated);
        }
    }
}
