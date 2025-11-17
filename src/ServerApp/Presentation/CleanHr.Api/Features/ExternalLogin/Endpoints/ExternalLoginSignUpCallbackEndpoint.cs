using System.Security.Claims;
using CleanHr.Api.Helpers;
using CleanHr.Application.Infrastructures;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CleanHr.Api.Features.ExternalLogin.Endpoints;

[ApiVersion("1.0")]
internal class ExternalLoginSignUpCallbackEndpoint(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    TokenManager tokenManager,
    ILogger<ExternalLoginSignUpCallbackEndpoint> logger,
    IExceptionLogger exceptionLogger) : ExternalLoginEndpointBase
{
    private string ClientLoginUrl => "https://localhost:44364/identity/login";

    private string ClientSignupUrl => "https://localhost:44364/identity/registration";

    private string ErrorMessage { get; set; }

    // GET: /Account/ExternalLoginSignUpCallback
    [HttpGet("sign-up-callback")]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginSignUpCallback(
        string returnUrl = null,
        string remoteError = null)
    {
        try
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectWithError(ErrorMessage);
            }

            ExternalLoginInfo externalLoginInfo = await signInManager.GetExternalLoginInfoAsync();

            if (externalLoginInfo == null)
            {
                ErrorMessage = "The invalid request.";
                return RedirectWithError(ErrorMessage);
            }

            string email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToPage("/ExternalLoginConfirmationPage");
            }

            ApplicationUser applicationUser = await userManager.FindByEmailAsync(email);

            if (applicationUser == null)
            {
                applicationUser = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
                IdentityResult userCreationResult = await userManager.CreateAsync(applicationUser);

                if (!userCreationResult.Succeeded)
                {
                    ErrorMessage = userCreationResult.Errors.Select(e => e.Description).FirstOrDefault();
                    return RedirectWithError(ErrorMessage);
                }

                applicationUser = await userManager.FindByEmailAsync(email);
            }

            IList<UserLoginInfo> externalLogins = await userManager.GetLoginsAsync(applicationUser);
            bool isExistent = externalLogins.Any(el => el.LoginProvider == externalLoginInfo.LoginProvider && el.ProviderKey == externalLoginInfo.ProviderKey);

            if (isExistent == false)
            {
                IdentityResult addExternalLoginResult = await userManager.AddLoginAsync(applicationUser, externalLoginInfo);

                if (!addExternalLoginResult.Succeeded)
                {
                    ErrorMessage = addExternalLoginResult.Errors.Select(e => e.Description).FirstOrDefault();
                    return RedirectWithError(ErrorMessage);
                }
            }

            // Sign in the user with this external login provider if the user already has a login.
            SignInResult signInResult = await signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: false);

            if (signInResult.Succeeded)
            {
                // Update any authentication tokens if login succeeded
                await signInManager.UpdateExternalAuthenticationTokensAsync(externalLoginInfo);

                logger.LogInformation(5, "User logged in with {Name} provider.", externalLoginInfo.LoginProvider);

                string jwt = await tokenManager.GetJwtTokenAsync(applicationUser);

                string redirectUrl = QueryHelpers.AddQueryString(ClientLoginUrl, "jwt", jwt);
                return Redirect(redirectUrl);
            }

            if (signInResult.RequiresTwoFactor)
            {
                ErrorMessage = "Require two factor authentication.";
            }
            else if (signInResult.IsLockedOut)
            {
                ErrorMessage = "This account has been locked out, please try again later.";
            }
            else
            {
                ErrorMessage = "The provided external login info is not valid.";
            }

            return RedirectWithError(ErrorMessage);
        }
        catch (Exception exception)
        {
            await exceptionLogger.LogAsync(exception);
            ErrorMessage = "There is a problem with service. Please try again. if the problem persists then contact with system admin.";
            return RedirectWithError(ErrorMessage);
        }
    }

    private RedirectResult RedirectWithError(string errorMessage)
    {
        string redirectUrl = QueryHelpers.AddQueryString(ClientSignupUrl, "error", errorMessage);
        return Redirect(redirectUrl);
    }
}
