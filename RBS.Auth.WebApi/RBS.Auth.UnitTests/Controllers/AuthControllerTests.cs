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
using RBS.Auth.UnitTests.TestExtensions;
using RBS.Auth.WebApi.Controllers;
using RBS.Auth.WebApi.Models;

namespace RBS.Auth.UnitTests.Controllers;

public class AuthControllerTests
{
    private Mock<IAuthenticateService> _authenticateServiceMock;
    private Mock<IJwtTokenService> _tokenServiceMock;
    private IOptions<AuthOptions> _options;
    private Mock<IObjectModelValidator> _objectValidator;

    [SetUp]
    public void Setup()
    {
        _authenticateServiceMock = new Mock<IAuthenticateService>();
        _tokenServiceMock = new Mock<IJwtTokenService>();
        _objectValidator = new Mock<IObjectModelValidator>();
        _options = Options.Create(new AuthOptions());

        _objectValidator.Setup(ObjectValidatorExtensions.GetValidatorExpression());
    }

    [Test]
    public void Login_RequestModelIsValid_ShouldReturn200StatusCode()
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
            .Returns(new UserCredential());

        _tokenServiceMock
            .Setup(t => t.Generate(It.IsAny<AuthOptions>(), It.IsAny<UserCredential>()))
            .Returns(token);

        var controller = new AuthController(_options,
            _tokenServiceMock.Object,
            _authenticateServiceMock.Object,
            MapperExtensions.Mapper);

        // Act
        var response = controller.Login(loginRequest);

        // Assert
        response.Should().NotBeNull();
        var objResult = response as ObjectResult;
        objResult?.StatusCode.Should().Be(200);
        objResult?.Value.Should().BeEquivalentTo(new { access_token = "token" });
    }

    [Test]
    public void Login_RequestModelIsValidUserNotFound_ShouldReturn401StatusCode()
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
            .Returns((UserCredential)null);

        var controller = new AuthController(_options,
            _tokenServiceMock.Object,
            _authenticateServiceMock.Object,
            MapperExtensions.Mapper);

        // Act
        var response = controller.Login(loginRequest);

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
            MapperExtensions.Mapper);

        _authenticateServiceMock
            .Setup(t => t.Register(It.IsAny<RegisterModel>()))
            .Returns(Task.FromResult(false));

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
            MapperExtensions.Mapper);

        _authenticateServiceMock
            .Setup(t => t.Register(It.IsAny<RegisterModel>()))
            .Returns(Task.FromResult(true));

        // Act
        var response = await controller.Register(registerRequest);
        var okResult = response as StatusCodeResult;

        // Assert
        okResult.Should().NotBeNull();
        okResult?.StatusCode.Should().Be(200);
    }
}