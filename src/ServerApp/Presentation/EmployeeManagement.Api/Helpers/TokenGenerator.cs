﻿using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EmployeeManagement.Api.Configs;
using EmployeeManagement.Application.Commands.IdentityCommands.UserCommands;
using EmployeeManagement.Application.Queries.IdentityQueries.UserQueries;
using EmployeeManagement.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Api.Helpers;

[ScopedService]
public class TokenManager
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMediator _mediator;
    private readonly JwtConfig _jwtConfig;

    public TokenManager(
        JwtConfig jwtConfig,
        UserManager<ApplicationUser> userManager,
        IMediator mediator)
    {
        _jwtConfig = jwtConfig;
        _userManager = userManager;
        _mediator = mediator;
    }

    public async Task<string> GetJwtTokenAsync(ApplicationUser user)
    {
        user.ThrowIfNull(nameof(user));

        IList<string> roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

        GetRefreshTokenQuery getRefreshTokenQuery = new GetRefreshTokenQuery(user.Id);

        RefreshToken refreshToken = await _mediator.Send(getRefreshTokenQuery);

        if (refreshToken == null)
        {
            string token = GetRefreshToken();
            StoreRefreshTokenCommand storeRefreshTokenCommand = new StoreRefreshTokenCommand(user.Id, token);
            refreshToken = await _mediator.Send(storeRefreshTokenCommand);
        }
        else
        {
            if (refreshToken.ExpireAtUtc < DateTime.UtcNow)
            {
                string token = GetRefreshToken();
                UpdateRefreshTokenCommand updateRefreshTokenCommand = new UpdateRefreshTokenCommand(user.Id, token);
                refreshToken = await _mediator.Send(updateRefreshTokenCommand);
            }
        }

        DateTime utcNow = DateTime.Now;

        string fullName = string.IsNullOrWhiteSpace(user.FullName) ? user.UserName : user.FullName;

        List<Claim> claims = new List<Claim>
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
            new Claim("rt", refreshToken.Token),
        };

        if (roles != null && roles.Any())
        {
            foreach (string item in roles)
            {
                claims.Add(new(ClaimTypes.Role, item));
            }
        }

        SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
        SigningCredentials signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwt = new JwtSecurityToken(
            signingCredentials: signingCredentials,
            claims: claims,
            notBefore: utcNow,
            expires: utcNow.AddSeconds(_jwtConfig.TokenLifeTime),
            audience: _jwtConfig.Issuer,
            issuer: _jwtConfig.Issuer);

        JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        jwtSecurityTokenHandler.OutboundClaimTypeMap.Clear();
        string accessToken = jwtSecurityTokenHandler.WriteToken(jwt);

        return accessToken;
    }

    public ClaimsPrincipal ParseExpiredToken(string accessToken)
    {
        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key)),
            ValidateLifetime = false
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        ClaimsPrincipal principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
        JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
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