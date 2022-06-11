using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RBS.Auth.Services.Interfaces.Verification;

namespace RBS.Auth.WebApi.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly IVerificationService _verificationService;

        public VerificationController(IVerificationService verificationService)
        {
            _verificationService = verificationService;
        }

        [HttpGet]
        public async Task<IActionResult> VerifyCode(Guid userId, string code)
        {
            if(await _verificationService.IsCodeValid(userId, code))
            {
                await _verificationService.VerifyUser(userId);

                return Ok();
            }

            return Unauthorized();
        }
    }
}
