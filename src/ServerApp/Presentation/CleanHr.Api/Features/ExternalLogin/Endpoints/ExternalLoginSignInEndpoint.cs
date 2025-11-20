using CleanHr.Domain.Aggregates.IdentityAggregate;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanHr.Api.Features.ExternalLogin.Endpoints;

[ApiVersion("1.0")]
public class ExternalLoginSignInEndpoint(
    SignInManager<ApplicationUser> signInManager) : ExternalLoginEndpointBase
{
    [HttpGet("sign-in")]
    public IActionResult Get(string provider)
    {
        // Request a redirect to the external login provider.
        string redirectUrl = "api/v1/external-login/sign-in-callback";
        AuthenticationProperties properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }
}
