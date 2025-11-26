using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CleanHr.Application.Commands.IdentityCommands.UserCommands;
using CleanHr.Application.Infrastructures;
using CleanHr.Application.Queries.IdentityQueries.UserQueries;
using CleanHr.Domain;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Services;

public class JwtTokenManager
{
    private readonly JwtConfig _jwtConfig;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMediator _mediator;

    public JwtTokenManager(
        JwtConfig jwtConfig,
        UserManager<ApplicationUser> userManager,
        IMediator mediator)
    {
        _jwtConfig = jwtConfig ?? throw new ArgumentNullException(nameof(jwtConfig));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<string> GetTokenAsync(string userId)
    {
        userId.ThrowIfNullOrEmpty(nameof(userId));

        ApplicationUser user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found.");
        }

        return await GetTokenAsync(user);
    }

    public async Task<(string AccessToken, string RefreshToken)> GetTokenAsync(string accessToken, string refreshToken)
    {
        accessToken.ThrowIfNull(nameof(accessToken));
        refreshToken.ThrowIfNull(nameof(refreshToken));

        ClaimsPrincipal claimsPrincipal = ParseExpiredToken(accessToken);
        string userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        // Get the refresh token from database
        GetRefreshTokenQuery getRefreshTokenQuery = new(refreshToken);
        RefreshToken currentToken = await _mediator.Send(getRefreshTokenQuery);

        if (currentToken == null)
        {
            throw new SecurityTokenException("Invalid refresh token.");
        }

        // Verify the token belongs to the user
        if (currentToken.UserId != Guid.Parse(userId))
        {
            throw new SecurityTokenException("Token does not belong to this user.");
        }

        // Check if token is valid (not revoked and not expired)
        if (!currentToken.IsValid())
        {
            // Token is invalid - check if it's a reuse attack
            if (currentToken.IsRevoked)
            {
                // Possible reuse attack - revoke entire token family
                RevokeRefreshTokenFamilyCommand revokeCommand = new(currentToken.TokenFamily);
                await _mediator.Send(revokeCommand);
                throw new SecurityTokenException("Token reuse detected. All tokens in this family have been revoked.");
            }

            throw new SecurityTokenException("Refresh token has expired.");
        }

        // Revoke the current token (it's been used)
        currentToken.Revoke();
        _mediator.Send(new UpdateTokenCommand(currentToken));

        // Generate new refresh token in the same family chain
        string newRefreshTokenString = GetRefreshToken();
        RotateRefreshTokenCommand rotateCommand = new(
            Guid.Parse(userId),
            newRefreshTokenString,
            currentToken.TokenFamily,
            currentToken.Id);

        Result<RefreshToken> rotateResult = await _mediator.Send(rotateCommand);

        if (rotateResult.IsSuccess == false)
        {
            throw new InvalidOperationException("Failed to rotate refresh token.");
        }

        ApplicationUser user = await _userManager.FindByIdAsync(userId);

        // Generate a new access token
        string newAccessToken = await GenerateAccessTokenAsync(user);

        return (newAccessToken, newRefreshTokenString);
    }

    public async Task<(string AccessToken, string RefreshToken)> GetTokensAsync(ApplicationUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        // For login, always create a new refresh token with a new token family
        // This starts a new token chain
        string tokenString = GetRefreshToken();
        StoreRefreshTokenCommand storeRefreshTokenCommand = new(user.Id, tokenString);
        Result<RefreshToken> storeResult = await _mediator.Send(storeRefreshTokenCommand);

        if (storeResult.IsSuccess == false)
        {
            throw new InvalidOperationException("Failed to store refresh token.");
        }

        RefreshToken refreshToken = storeResult.Value;

        // Generate access token
        string accessToken = await GenerateAccessTokenAsync(user);

        return (accessToken, refreshToken.Token);
    }

    private async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        IList<string> roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

        DateTime utcNow = DateTime.UtcNow;

        string fullName = string.IsNullOrWhiteSpace(user.FullName) ? user.UserName : user.FullName;

        // Build claims for the access token. Do NOT include the raw refresh token here.
        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, fullName),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.GivenName, fullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)),
        };

        if (roles != null && roles.Any())
        {
            foreach (string item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }
        }

        SymmetricSecurityKey signingKey = new(Encoding.UTF8.GetBytes(_jwtConfig.Key));
        SigningCredentials signingCredentials = new(signingKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwt = new(
            signingCredentials: signingCredentials,
            claims: claims,
            notBefore: utcNow,
            expires: utcNow.AddSeconds(_jwtConfig.TokenLifeTime),
            audience: _jwtConfig.Issuer,
            issuer: _jwtConfig.Issuer);

        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        jwtSecurityTokenHandler.OutboundClaimTypeMap.Clear();
        string accessToken = jwtSecurityTokenHandler.WriteToken(jwt);

        return accessToken;
    }

    public async Task<string> GetTokenAsync(ApplicationUser user)
    {
        (string accessToken, string _) = await GetTokensAsync(user);
        return accessToken;
    }

    public ClaimsPrincipal ParseExpiredToken(string accessToken)
    {
        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key)),
            // Allow parsing expired tokens here so we can extract claims from an expired access token
            // during the refresh-flow. Do not accept an expired token as valid authentication.
            ValidateLifetime = false
        };

        JwtSecurityTokenHandler tokenHandler = new();
        ClaimsPrincipal principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenException("Invalid access token.");
        }

        return principal;
    }

    private string GetRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);

        string refreshToken = Convert.ToBase64String(randomNumber);

        return refreshToken;
    }
}
