using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using RBS.Auth.Common;
using RBS.Auth.Db.Domain;
using RBS.Auth.Services.Interfaces.Tokens;

namespace RBS.Auth.Services.Tokens;

public class JwtTokenService : IJwtTokenService
{
    public string Generate(AuthOptions authOptions, UserCredential user)
    {
        var securityKey = authOptions.GetSymmetricSecurityKey();
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Email, user.Details.Email),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString())
        };

        var tokenLifetime = DateTime.Now.AddSeconds(authOptions.TokenLifetime);

        var token = new JwtSecurityToken(authOptions.Issuer,
            authOptions.Audience,
            claims,
            expires: tokenLifetime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}