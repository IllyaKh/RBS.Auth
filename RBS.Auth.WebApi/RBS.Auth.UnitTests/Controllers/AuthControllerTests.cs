using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RBS.Auth.Common;
using RBS.Auth.Common.Models;
using RBS.Auth.Db.Domain;
using RBS.Auth.Services.Hashing.Options;
using RBS.Auth.Services.Interfaces.Authenticate;
using RBS.Auth.Services.Interfaces.Tokens;
using RBS.Auth.Services.Interfaces.Verification;
using RBS.Auth.UnitTests.TestExtensions;
using RBS.Auth.WebApi.Controllers;
using RBS.Auth.WebApi.Models;

namespace RBS.Auth.UnitTests.Controllers;

public class AuthControllerTests
{
    private Mock<IAuthenticateService> _authenticateServiceMock;
    private Mock<IJwtTokenService> _tokenServiceMock;
    private Mock<IVerificationService> _verificationServiceMock;

    private IOptions<AuthOptions> _options;
    private Mock<IObjectModelValidator> _objectValidator;

    [SetUp]
    public void Setup()
    {
        _authenticateServiceMock = new Mock<IAuthenticateService>();
        _tokenServiceMock = new Mock<IJwtTokenService>();
        _objectValidator = new Mock<IObjectModelValidator>();
        _verificationServiceMock = new Mock<IVerificationService>();
        _options = Options.Create(new AuthOptions());

        _objectValidator.Setup(ObjectValidatorExtensions.GetValidatorExpression());
    }

    [Test]
    public async Task Login_RequestModelIsValid_ShouldReturn200StatusCode()
    {
        // Arrange
        var loginRequest = new LoginRequest()
        {
            Email = "mail@mail.com",
            Password = "pass"
        };

        var token = "token";

        _authenticateServiceMock
            .Setup(t => t.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult(new UserCredential()));

        _tokenServiceMock
            .Setup(t => t.Generate(It.IsAny<UserCredential>()))
            .Returns(token);

        var controller = new AuthController(_options,
            _tokenServiceMock.Object,
            _authenticateServiceMock.Object,
            _verificationServiceMock.Object,
            MapperExtensions.Mapper);

        // Act
        var response = await controller.Login(loginRequest);

        // Assert
        response.Should().NotBeNull();
        var objResult = response as ObjectResult;
        objResult?.StatusCode.Should().Be(200);
        objResult?.Value.Should().BeEquivalentTo(new { access_token = "token" });
    }

    [Test]
    public async Task Login_RequestModelIsValidUserNotFound_ShouldReturn401StatusCode()
    {
        // Arrange
        var loginRequest = new LoginRequest()
        {
            Email = "mail@mail.com",
            Password = "pass"
        };

        var token = "token";

        _authenticateServiceMock
            .Setup(t => t.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult((UserCredential)null));

        var controller = new AuthController(_options,
            _tokenServiceMock.Object,
            _authenticateServiceMock.Object,
            _verificationServiceMock.Object,
            MapperExtensions.Mapper);

        // Act
        var response = await controller.Login(loginRequest);

        // Assert
        response.Should().NotBeNull();
        var objResult = response as StatusCodeResult;
        objResult.Should().NotBeNull();
        objResult?.StatusCode.Should().Be(401);
    }

    [Test]
    public async Task Register_AuthenticateServiceReturnFalse_ShouldReturn500StatusCode()
    {
        // Arrange
        var registerRequest = new RegisterRequest();

        var controller = new AuthController(_options,
            _tokenServiceMock.Object,
            _authenticateServiceMock.Object,
            _verificationServiceMock.Object,
            MapperExtensions.Mapper);

        // Act
        var response = await controller.Register(registerRequest);

        // Assert
        response.Should().NotBeNull();
        var objResult = response as StatusCodeResult;
        objResult.Should().NotBeNull();
        objResult?.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task Register_AuthenticateServiceReturnTrue_ShouldReturn200StatusCode()
    {
        // Arrange
        var registerRequest = new RegisterRequest();

        var controller = new AuthController(_options,
            _tokenServiceMock.Object,
            _authenticateServiceMock.Object,
            _verificationServiceMock.Object,
            MapperExtensions.Mapper);

        _authenticateServiceMock
            .Setup(t => t.Register(It.IsAny<RegisterModel>()))
            .Returns(Task.FromResult(new UserCredential()));

        // Act
        var response = await controller.Register(registerRequest);
        var okResult = response as StatusCodeResult;

        // Assert
        okResult.Should().NotBeNull();
        okResult?.StatusCode.Should().Be(200);
    }
}