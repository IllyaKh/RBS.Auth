using Microsoft.IdentityModel.Tokens;
using RBS.Auth.Common;
using RBS.Auth.Common.Models;
using RBS.Auth.Services.Interfaces.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace RBS.Auth.Services.Tokens
{
    public class JWTTokenService : IJWTTokenService
    {
        public string Generate(AuthOptions authOptions, Account user)
        {
            var securityKey = authOptions.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };

            claims.AddRange(user.Roles.Select(r => new Claim("role", r.ToString())));

            var tokenLifetime = DateTime.Now.AddSeconds(authOptions.TokenLifetime);

            var token = new JwtSecurityToken(authOptions.Issuer,
                authOptions.Audience,
                claims,
                expires: tokenLifetime,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
