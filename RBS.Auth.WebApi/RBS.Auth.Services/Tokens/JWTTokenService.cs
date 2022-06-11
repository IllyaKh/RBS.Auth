using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RBS.Auth.Common;
using RBS.Auth.Db.Domain;
using RBS.Auth.Services.Interfaces.Tokens;

namespace RBS.Auth.Services.Tokens;

public class JwtTokenService : IJwtTokenService
{
    private readonly AuthOptions _authOptions;

    public JwtTokenService(IOptions<AuthOptions> authOptions)
    {
        _authOptions = authOptions.Value;
    }

    public string Generate(UserCredential user)
    {
        var securityKey = _authOptions.GetSymmetricSecurityKey();
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("mail", user.Details.Email),
            new("id", user.Id.ToString())
        };

        if (user.Claims != null)
        {
            foreach (var claim in user.Claims)
            {
                claims.Add(new Claim(claim.Name, claim.Role.ToString()));
            }
        }

        var tokenLifetime = DateTime.Now.AddSeconds(_authOptions.TokenLifetime);

        var token = new JwtSecurityToken(_authOptions.Issuer,
            _authOptions.Audience,
            claims,
            expires: tokenLifetime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public IDictionary<string ,string> GetClaims(string token)
    {
        var claims = GetClaimsPrincipal(token);

        if (claims != null)
        {
            var claimList = claims.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value));

            return claimList.ToDictionary(c => c.Key, c => c.Value);
        }

        return null;
    }

    public bool ValidateToken(string token)
    {
        if (GetClaimsPrincipal(token) != null)
        {
            return true;
        }

        return false;
    }

    private ClaimsPrincipal GetClaimsPrincipal(string token)
    {
        try
        {
            SecurityToken validatedToken;
            return new JwtSecurityTokenHandler().ValidateToken(token, GetValidationParameters(), out validatedToken);
        }
        catch
        {
            return null;
        }
    }

    private TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters()
        {
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = _authOptions.Issuer,
            ValidAudience = _authOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.Secret))
        };
    }
}