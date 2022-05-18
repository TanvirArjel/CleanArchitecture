using Identity.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Endpoints.ExternalLoginEndpoints;

[ApiVersion("1.0")]
public class ExternalLoginSignUpEndpoint : ExternalLoginEndpointBase
{
    private readonly SignInManager<User> _signInManager;

    public ExternalLoginSignUpEndpoint(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpGet("sign-up")]
    public IActionResult Get(string provider, string returnUrl = null)
    {
        // Request a redirect to the external login provider.
        string redirectUrl = $"api/v1/external-login/sign-up-callback?returnUrl={returnUrl}";
        AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }
}
