using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Application.Implementations.Services
{
    internal class TokenGenerator : ITokenGenerator
    {
        private readonly JwtConfig _jwtConfig;

        public TokenGenerator(JwtConfig jwtConfig)
        {
            _jwtConfig = jwtConfig;
        }

        public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles)
        {
            DateTime utcNow = DateTime.UtcNow;

            string fullName = applicationUser.FirstName + " " + applicationUser.LastName;

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, applicationUser.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, fullName),
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sid, applicationUser.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, applicationUser.UserName),
                new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, fullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString(CultureInfo.InvariantCulture))
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
    }
}
