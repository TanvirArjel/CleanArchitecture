using System.Security.Claims;
using System.Threading;
using CleanHr.Api.Features.User.Models;
using CleanHr.Api.Helpers;
using CleanHr.Application.Queries.IdentityQueries.UserQueries;
using FluentValidation;
using MediatR;

namespace CleanHr.Api.Features.User.Validators;

internal sealed class TokenRefreshModelValidator : AbstractValidator<TokenRefreshModel>
{
    private readonly TokenManager _tokenManager;
    private readonly IMediator _mediator;

    private string _userId = string.Empty;

    public TokenRefreshModelValidator(TokenManager tokenManager, IMediator mediator)
    {
        _tokenManager = tokenManager;

        RuleFor(trm => trm.AccessToken).NotEmpty()
                                       .WithMessage("The accessToken is required.")
                                       .Must(IsAccessTokenValid)
                                       .WithMessage("The accessToken is invalid.");

        RuleFor(trm => trm.RefreshToken).NotEmpty()
                                       .WithMessage("The refreshToken is required.")
                                       .MustAsync(IsRefreshTokenValidAsync)
                                       .WithMessage("The refreshToken is not valid.");
        _mediator = mediator;
    }

    private bool IsAccessTokenValid(
        string accessToken)
    {
        ClaimsPrincipal claimsPrincipal;

        try
        {
            claimsPrincipal = _tokenManager.ParseExpiredToken(accessToken);
        }
        catch (Exception)
        {
            return false;
        }

        _userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        return _userId != null;
    }

    private async Task<bool> IsRefreshTokenValidAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        IsRefreshTokenValidQuery isRefreshTokenValidQuery = new(Guid.Parse(_userId), refreshToken);

        bool isValid = await _mediator.Send(isRefreshTokenValidQuery, cancellationToken);

        return isValid;
    }
}
