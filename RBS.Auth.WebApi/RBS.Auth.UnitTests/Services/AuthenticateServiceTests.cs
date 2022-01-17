using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RBS.Auth.Common.Models;
using RBS.Auth.Db;
using RBS.Auth.Db.Domain;
using RBS.Auth.Services.Authenticate;
using RBS.Auth.Services.Interfaces.Hashing;
using RBS.Auth.UnitTests.TestExtensions;

namespace RBS.Auth.UnitTests.Services;

public class AuthenticateServiceTests
{
    private DbContextOptions<AuthContext> _dbOptions;
    private Mock<ILogger<AuthenticateService>> _loggerMock;
    private Mock<IHashingService> _hashingServiceMock;
    private string userEmail;
    private string userPassword;
    private string testHashSalt;
    
    [SetUp]
    public void Setup()
    {
        _dbOptions = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _loggerMock = new Mock<ILogger<AuthenticateService>>();
        _hashingServiceMock = new Mock<IHashingService>();
        userEmail = "user@mail.com";
        userPassword = "pass";
        testHashSalt = "test";
    }

    [Test]
    public void Authenticate_UserNotFound_ShouldReturnNull()
    {
        // Arrange
        _hashingServiceMock.Setup(x => 
                x.Check(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        using var context = new AuthContext(_dbOptions);

        var authService = new AuthenticateService(context, _hashingServiceMock.Object, _loggerMock.Object);

        // Act 
        var user = authService.Authenticate("12","12");
            
        // Assert
        user.Should().BeNull();
    }

    [Test]
    public void Authenticate_UserExists_ShouldReturnUser()
    {
        // Arrange
        _hashingServiceMock.Setup(x =>
                x.Check(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        using var context = new AuthContext(_dbOptions);

        var userDetails = new UserDetails() {Email = userEmail};
        context.Details.Add(userDetails);
        context.UserCredentials.Add(new UserCredential() { Details = userDetails });

        context.SaveChanges();

        var authService = new AuthenticateService(context, _hashingServiceMock.Object, _loggerMock.Object);

        // Act 
        var user = authService.Authenticate(userEmail, userPassword);

        // Assert
        user.Should().NotBeNull();
        user.Details.Email.Should().Be(userEmail);

        context.Dispose();
    }


    [Test]
    public void Authenticate_HashingServiceThrowsException_ShouldLogExceptionWithMessage()
    {
        // Arrange
        var exception = new Exception("");

        _hashingServiceMock.Setup(x =>
                x.Check(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Throws(exception);

        using var context = new AuthContext(_dbOptions);

        var authService = new AuthenticateService(context, _hashingServiceMock.Object, _loggerMock.Object);

        var userDetails = new UserDetails() { Email = userEmail };
        context.Details.Add(userDetails);
        context.UserCredentials.Add(new UserCredential() { Details = userDetails });

        context.SaveChanges();

        // Act 
        authService.Authenticate(userEmail, userPassword);

        // Assert

        _loggerMock.Verify(LoggerExtensions<AuthenticateService>
            .VerifyLogging($"An error occurred when login user: {userEmail}", LogLevel.Error), Times.Once);

        context.Dispose();
    }

    [Test]
    public void Authenticate_HashingServiceThrowsException_ShouldReturnNull()
    {
        // Arrange
        var exception = new Exception("");

        _hashingServiceMock.Setup(x =>
                x.Check(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Throws(exception);

        using var context = new AuthContext(_dbOptions);

        var authService = new AuthenticateService(context, _hashingServiceMock.Object, _loggerMock.Object);

        var userDetails = new UserDetails() { Email = userEmail };
        context.Details.Add(userDetails);
        context.UserCredentials.Add(new UserCredential() { Details = userDetails });

        context.SaveChanges();

        // Act 
        var user = authService.Authenticate(userEmail, userPassword);

        // Assert
        user.Should().BeNull();

        context.Dispose();
    }

    [Test]
    public async Task Register_HashingServiceThrowsException_ShouldReturnFalse()
    {
        // Arrange
        var registerModel = new RegisterModel() { Email = userEmail, Password = userPassword };
        var exception = new Exception("");

        _hashingServiceMock.Setup(x =>
                x.Hash(It.IsAny<string>()))
            .Throws(exception);

        await using var context = new AuthContext(_dbOptions);

        var authService = new AuthenticateService(context, _hashingServiceMock.Object, _loggerMock.Object);

        // Act 
        var user = await authService.Register(registerModel);

        // Assert
        user.Should().BeFalse();

        await context.DisposeAsync();
    }

    [Test]
    public async Task Register_HashingServiceThrowsException_ShouldLogExceptionWithMessage()
    {
        // Arrange
        var registerModel = new RegisterModel() { Email = userEmail, Password = userPassword };
        var exception = new Exception("");

        _hashingServiceMock.Setup(x =>
                x.Hash(It.IsAny<string>()))
            .Throws(exception);

        await using var context = new AuthContext(_dbOptions);

        var authService = new AuthenticateService(context, _hashingServiceMock.Object, _loggerMock.Object);

        // Act 
        await authService.Register(registerModel);

        // Assert
        _loggerMock.Verify(LoggerExtensions<AuthenticateService>
            .VerifyLogging($"An error occurred when register user: {userEmail}", LogLevel.Error), Times.Once);

        await context.DisposeAsync();
    }

    [Test]
    public async Task Register_HasCorrectRegisterModel_ShouldRegisterUser()
    {
        // Arrange
        var registerModel = new RegisterModel() { Email = userEmail, Password = userPassword };

        _hashingServiceMock.Setup(x =>
                x.Hash(It.IsAny<string>()))
            .Returns((testHashSalt, testHashSalt));

        await using var context = new AuthContext(_dbOptions);

        var authService = new AuthenticateService(context, _hashingServiceMock.Object, _loggerMock.Object);

        // Act 
        var registerResult = await authService.Register(registerModel);

        var userCredentials = context.UserCredentials
            .Include(d => d.Claims)
            .Include(d => d.Details)
            .FirstOrDefault(x => x.Details.Email == userEmail);

        // Assert
        userCredentials.Should().NotBeNull();
        userCredentials?.Details.Should().NotBeNull();
        userCredentials?.Details.Email.Should().Be(userEmail);
        userCredentials?.Hash.Should().Be(testHashSalt);
        userCredentials?.Salt.Should().Be(testHashSalt);

        registerResult.Should().BeTrue();

        await context.DisposeAsync();
    }
}