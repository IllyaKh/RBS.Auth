using System.Collections.Generic;
using RBS.Auth.Common;
using RBS.Auth.Db.Domain;

namespace RBS.Auth.Services.Interfaces.Tokens;

public interface IJwtTokenService
{
    string Generate(UserCredential user);

    IDictionary<string, string> GetClaims(string token);

    bool ValidateToken(string token);

}