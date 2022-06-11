using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RBS.Auth.Common;
using RBS.Auth.Common.Models;
using RBS.Auth.Services.Interfaces.Authenticate;
using RBS.Auth.Services.Interfaces.Tokens;
using RBS.Auth.Services.Interfaces.Verification;
using RBS.Auth.WebApi.Models;

namespace RBS.Auth.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticateService _authenticateService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IVerificationService _verificationService;
    private readonly IMapper _mapper;

    public AuthController(IOptions<AuthOptions> authOptions,
        IJwtTokenService jwtTokenService,
        IAuthenticateService authenticateService,
        IVerificationService verificationService,
        IMapper mapper)
    {
        _jwtTokenService = jwtTokenService;
        _authenticateService = authenticateService;
        _verificationService = verificationService;
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

        var token = _jwtTokenService.Generate(user);

        return string.IsNullOrEmpty(token) ? 
            StatusCode(500) : 
            Ok(new { access_token = token, is_verified = user.IsVerified });

    }

    [HttpPut]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return StatusCode(403);

        var registerModel = _mapper.Map<RegisterModel>(request);

        var user = await _authenticateService.Register(registerModel);
        
        if (user != null)
        {
            var code = await _verificationService.GenerateCode(user.Id);

            await _verificationService.SendVerifyCode(request.Email, code);

            return Ok(new { user_id = user.Id });
        }

        return StatusCode(500);
    }

    [HttpGet]
    public IActionResult IsLogedIn(string token)
    {
        var isValid = _jwtTokenService.ValidateToken(token);

        return isValid ? Ok() : Unauthorized();
    }

    [HttpGet]
    public IActionResult GetClaims(string token)
    {
        var claims = _jwtTokenService.GetClaims(token);

        return Ok(claims);
    }
}