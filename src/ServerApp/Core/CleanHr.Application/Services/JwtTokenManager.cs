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

    public async Task<string> GetTokenAsync(string accessToken, string refreshToken)
    {
        accessToken.ThrowIfNull(nameof(accessToken));

        ClaimsPrincipal claimsPrincipal = ParseExpiredToken(accessToken);
        string userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        IsRefreshTokenValidQuery isRefreshTokenValidQuery = new(Guid.Parse(userId), refreshToken);

        bool isValid = await _mediator.Send(isRefreshTokenValidQuery);

        ApplicationUser user = await _userManager.FindByIdAsync(userId);
        return await GetTokenAsync(user);
    }

    public async Task<string> GetTokenAsync(ApplicationUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        IList<string> roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

        GetRefreshTokenQuery getRefreshTokenQuery = new(user.Id);

        RefreshToken refreshToken = await _mediator.Send(getRefreshTokenQuery);

        if (refreshToken == null)
        {
            string token = GetRefreshToken();
            StoreRefreshTokenCommand storeRefreshTokenCommand = new(user.Id, token);
            Result<RefreshToken> storeResult = await _mediator.Send(storeRefreshTokenCommand);

            if (storeResult.IsSuccess == false)
            {
                throw new InvalidOperationException("Failed to store refresh token.");
            }

            refreshToken = storeResult.Value;
        }
        else
        {
            if (refreshToken.ExpireAtUtc < DateTime.UtcNow)
            {
                string token = GetRefreshToken();
                UpdateRefreshTokenCommand updateRefreshTokenCommand = new(user.Id, token);
                Result<RefreshToken> updateResult = await _mediator.Send(updateRefreshTokenCommand);

                if (updateResult.IsSuccess == false)
                {
                    throw new InvalidOperationException("Failed to update refresh token.");
                }

                refreshToken = updateResult.Value;
            }
        }

        DateTime utcNow = DateTime.Now;

        string fullName = string.IsNullOrWhiteSpace(user.FullName) ? user.UserName : user.FullName;

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, fullName),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.GivenName, fullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)),
            new Claim("rt", refreshToken.Token),
        ];

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
        string newAccessToken = jwtSecurityTokenHandler.WriteToken(jwt);

        return newAccessToken;
    }

    public ClaimsPrincipal ParseExpiredToken(string accessToken)
    {
        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key)),
            ValidateLifetime = true
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
