using System.Security.Claims;
using CleanHr.Application.Services;
using CleanHr.Application.Extensions;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CleanHr.Api.Features.ExternalLogin.Endpoints;

[ApiVersion("1.0")]
public class ExternalLoginSignUpCallbackEndpoint : ExternalLoginEndpointBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtTokenManager _jwtTokenManager;
    private readonly ILogger<ExternalLoginSignUpCallbackEndpoint> _logger;

    public ExternalLoginSignUpCallbackEndpoint(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        JwtTokenManager jwtTokenManager,
        ILogger<ExternalLoginSignUpCallbackEndpoint> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtTokenManager = jwtTokenManager;
        _logger = logger;
    }

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

            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

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

            ApplicationUser applicationUser = await _userManager.FindByEmailAsync(email);

            if (applicationUser == null)
            {
                applicationUser = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
                IdentityResult userCreationResult = await _userManager.CreateAsync(applicationUser);

                if (!userCreationResult.Succeeded)
                {
                    ErrorMessage = userCreationResult.Errors.Select(e => e.Description).FirstOrDefault();
                    return RedirectWithError(ErrorMessage);
                }

                applicationUser = await _userManager.FindByEmailAsync(email);
            }

            IList<UserLoginInfo> externalLogins = await _userManager.GetLoginsAsync(applicationUser);
            bool isExistent = externalLogins.Any(el => el.LoginProvider == externalLoginInfo.LoginProvider && el.ProviderKey == externalLoginInfo.ProviderKey);

            if (isExistent == false)
            {
                IdentityResult addExternalLoginResult = await _userManager.AddLoginAsync(applicationUser, externalLoginInfo);

                if (!addExternalLoginResult.Succeeded)
                {
                    ErrorMessage = addExternalLoginResult.Errors.Select(e => e.Description).FirstOrDefault();
                    return RedirectWithError(ErrorMessage);
                }
            }

            // Sign in the user with this external login provider if the user already has a login.
            SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: false);

            if (signInResult.Succeeded)
            {
                // Update any authentication tokens if login succeeded
                await _signInManager.UpdateExternalAuthenticationTokensAsync(externalLoginInfo);

                _logger.LogWithLevel(LogLevel.Information, $"User logged in with {externalLoginInfo.LoginProvider} provider.");

                string jwt = await _jwtTokenManager.GetTokenAsync(applicationUser);

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
            Dictionary<string, object> fields = new()
            {
                { "ReturnUrl", returnUrl },
                { "RemoteError", remoteError }
            };
            _logger.LogException(exception, "An error occurred during external login sign-up callback.", fields);
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
