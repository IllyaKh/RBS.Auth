using RBS.Auth.Common;
using RBS.Auth.Db.Domain;

namespace RBS.Auth.Services.Interfaces.Tokens;

public interface IJwtTokenService
{
    string Generate(AuthOptions authOptions, UserCredential user);
}