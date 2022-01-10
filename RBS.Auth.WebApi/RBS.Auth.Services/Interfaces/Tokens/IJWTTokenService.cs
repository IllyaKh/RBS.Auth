using RBS.Auth.Common;
using RBS.Auth.Common.Models;

namespace RBS.Auth.Services.Interfaces.Tokens
{
    public interface IJWTTokenService
    {
        string Generate(AuthOptions authOptions, Account user);
    }
}
