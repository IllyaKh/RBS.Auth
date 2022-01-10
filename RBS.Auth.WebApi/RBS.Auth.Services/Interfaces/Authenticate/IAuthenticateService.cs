using RBS.Auth.Common.Models;

namespace RBS.Auth.Services.Interfaces.Authenticate
{
    public interface IAuthenticateService
    {
        Account Authenticate(string email, string password);
    }
}
