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

[Route("api/[controller]/[action]")]
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

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (!ModelState.IsValid) 
            return StatusCode(403);

        var user = await _authenticateService.Authenticate(request.Email, request.Password);

        if (user == null)
            return Unauthorized();

        var token = _jwtTokenService.Generate(_authOptions.Value, user);

        return string.IsNullOrEmpty(token) ? 
            StatusCode(500) : 
            Ok(new { access_token = token });

    }

    [HttpPut]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return StatusCode(403);

        var registerModel = _mapper.Map<RegisterModel>(request);

        if (await _authenticateService.Register(registerModel)) 
            return Ok();

        return StatusCode(500);
    }
}