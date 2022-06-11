using System;
using System.Threading.Tasks;

namespace RBS.Auth.Services.Interfaces.Verification
{
    public interface IVerificationService
    {
        Task<bool> IsCodeValid(Guid userId, string code);

        Task<string> GenerateCode(Guid userId);

        Task VerifyUser(Guid userId);

        Task SendVerifyCode(string email, string code);
    }
}
