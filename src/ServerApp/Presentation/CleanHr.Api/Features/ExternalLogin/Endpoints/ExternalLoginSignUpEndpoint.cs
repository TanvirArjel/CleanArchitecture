using CleanHr.Domain.Aggregates.IdentityAggregate;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanHr.Api.Features.ExternalLogin.Endpoints;

[ApiVersion("1.0")]
public class ExternalLoginSignUpEndpoint(
    SignInManager<ApplicationUser> signInManager) : ExternalLoginEndpointBase
{
    [HttpGet("sign-up")]
    public IActionResult Get(string provider, string returnUrl = null)
    {
        // Request a redirect to the external login provider.
        string redirectUrl = $"api/v1/external-login/sign-up-callback?returnUrl={returnUrl}";
        AuthenticationProperties properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }
}
