using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RBS.Auth.Common;
using RBS.Auth.Services.Interfaces.Authenticate;
using RBS.Auth.Services.Interfaces.Tokens;
using RBS.Auth.WebApi.Models;

namespace RBS.Auth.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<AuthOptions> _authOptions;
        private readonly IJWTTokenService _jwtTokenService;
        private readonly IAuthenticateService _authenticateService;

        public AuthController(IOptions<AuthOptions> authOptions,
            IJWTTokenService jwtTokenService,
            IAuthenticateService authenticateService)
        {
            _authOptions = authOptions;
            _jwtTokenService = jwtTokenService;
            _authenticateService = authenticateService;
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Login(LoginModel request)
        {
            var user = _authenticateService.Authenticate(request.Email, request.Password);

            if (user != null)
            {
                var token = _jwtTokenService.Generate(_authOptions.Value, user);

                return Ok(new
                {
                    access_token = token
                });
            }

            return Unauthorized();
        }
    }
}
