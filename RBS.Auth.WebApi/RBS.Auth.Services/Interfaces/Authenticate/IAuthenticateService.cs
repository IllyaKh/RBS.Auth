using System.Threading.Tasks;
using RBS.Auth.Common.Models;
using RBS.Auth.Db.Domain;

namespace RBS.Auth.Services.Interfaces.Authenticate;

public interface IAuthenticateService
{
    UserCredential Authenticate(string email, string password);

    Task<bool> Register(RegisterModel model);
}