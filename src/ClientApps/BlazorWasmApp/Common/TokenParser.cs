using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace BlazorWasmApp.Common
{
    [ScopedService]
    public class JwtTokenParser
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public JwtTokenParser(JwtSecurityTokenHandler jwtSecurityTokenHandler)
        {
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Not applicable here.")]
        public ClaimsPrincipal GetClaimsPrincipal(string token)
        {
            token.ThrowIfNullOrEmpty(nameof(token));

            JwtSecurityToken jwtToken = new JwtSecurityToken(token);

            List<Claim> microsoftClaims = new List<Claim>();

            foreach (Claim item in jwtToken.Claims)
            {
                switch (item.Type)
                {
                    case JwtRegisteredClaimNames.NameId:
                        microsoftClaims.Add(new Claim(ClaimTypes.NameIdentifier, item.Value));
                        break;
                    case JwtRegisteredClaimNames.Name:
                        microsoftClaims.Add(new Claim(ClaimTypes.Name, item.Value));
                        break;
                    case JwtRegisteredClaimNames.GivenName:
                        microsoftClaims.Add(new Claim(ClaimTypes.GivenName, item.Value));
                        break;
                    case JwtRegisteredClaimNames.Sub:
                        microsoftClaims.Add(new Claim(ClaimTypes.NameIdentifier, item.Value));
                        break;
                    case JwtRegisteredClaimNames.Email:
                        microsoftClaims.Add(new Claim(ClaimTypes.Email, item.Value));
                        break;
                    case JwtRegisteredClaimNames.Iat:
                        microsoftClaims.Add(new Claim(ClaimTypes.Expiration, item.Value));
                        break;
                    case JwtRegisteredClaimNames.Jti:
                        microsoftClaims.Add(new Claim(ClaimTypes.Sid, item.Value));
                        break;
                    default:
                        break;
                }
            }


            ClaimsIdentity identity = new ClaimsIdentity(microsoftClaims, "ServerAuth");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

            return claimsPrincipal;
        }

        public ClaimsPrincipal Parse(string token)
        {
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidIssuer = "SampleApp",

                ValidateAudience = false,
                ValidAudience = "SampleApp",

                ValidateIssuerSigningKey = false,
                //IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey)),
                //comment this and add this line to fool the validation logic
                SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                {
                    JwtSecurityToken jwt = new JwtSecurityToken(token);

                    return jwt;
                },

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero,
            };

            validationParameters.RequireSignedTokens = false;

            _jwtSecurityTokenHandler.InboundClaimTypeMap[JwtRegisteredClaimNames.Name] = ClaimTypes.Name;

            ClaimsPrincipal claimsPrincipal = _jwtSecurityTokenHandler
                .ValidateToken(token, validationParameters, out SecurityToken securityToken);

            return claimsPrincipal;
        }
    }
}
