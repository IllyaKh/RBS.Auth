using FluentAssertions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using RBS.Auth.Services.Hashing;
using RBS.Auth.Services.Hashing.Options; 

namespace RBS.Auth.UnitTests.Services;

public class HashingServiceTests
{
    private HashingService _hashingService;
    private string _password;

    [SetUp]
    public void Setup()
    {
        var options = Options.Create(new HashingOptions());
        _hashingService = new HashingService(options);

        _password = "pass";
    }

    [Test]
    public void Hash_HashOnePassword_HashAndSaltShouldNotBeNull()
    {
        // Arrange

        // Act 
        var result = _hashingService.Hash(_password);

        // Assert
        result.key.Should().NotBeNullOrEmpty();
        result.salt.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void Hash_HashTwoPasswords_KeyShouldNotBeEqual()
    {
        // Arrange

        // Act 
        var resultFirst = _hashingService.Hash(_password);
        var resultSecond = _hashingService.Hash(_password);

        // Assert
        resultFirst.Should().NotBeSameAs(resultSecond);
    }

    [Test]
    public void Check_PasswordHashSaltIsCorrect_ShouldReturnTrue()
    {
        // Arrange
        var resultFirst = _hashingService.Hash(_password);

        // Act 
        var isVerified = _hashingService.Check(resultFirst.key, resultFirst.salt, _password);

        // Assert
        isVerified.Should().BeTrue();
    }

    [Test]
    public void Check_PasswordIsIncorrect_ShouldReturnFalse()
    {
        // Arrange
        var incorrectPassword = _password + "1";
        var resultFirst = _hashingService.Hash(_password);

        // Act 
        var isVerified = _hashingService.Check(resultFirst.key, resultFirst.salt, incorrectPassword);

        // Assert
        isVerified.Should().BeFalse();
    }
}