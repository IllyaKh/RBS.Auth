using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RBS.Auth.Common;
using RBS.Auth.Common.Models;
using RBS.Auth.Services.Interfaces.Authenticate;
using RBS.Auth.Services.Interfaces.Tokens;
using RBS.Auth.WebApi.Models;

namespace RBS.Auth.WebApi.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticateService _authenticateService;
    private readonly IOptions<AuthOptions> _authOptions;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;

    public AuthController(IOptions<AuthOptions> authOptions,
        IJwtTokenService jwtTokenService,
        IAuthenticateService authenticateService,
        IMapper mapper)
    {
        _authOptions = authOptions;
        _jwtTokenService = jwtTokenService;
        _authenticateService = authenticateService;
        _mapper = mapper;
    }

    [Route("login")]
    [HttpPost]
    public IActionResult Login(LoginRequest request)
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

    [Route("register")]
    [HttpPut]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var registerModel = _mapper.Map<RegisterModel>(request);

        if (await _authenticateService.Register(registerModel)) return Ok();

        return StatusCode(500);
    }
}