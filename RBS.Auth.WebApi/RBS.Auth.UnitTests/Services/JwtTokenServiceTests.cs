using System;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using RBS.Auth.Common;
using RBS.Auth.Db.Domain;
using RBS.Auth.Services.Hashing;
using RBS.Auth.Services.Hashing.Options;
using RBS.Auth.Services.Tokens;

namespace RBS.Auth.UnitTests.Services;

public class JwtTokenServiceTests
{
    private AuthOptions _authOptions;
    private UserCredential _credentials;

    [SetUp]
    public void Setup()
    {
        _authOptions = new AuthOptions() 
        {
            Audience = "resourceServer",
            Issuer = "authServer",
            Secret = "123gegergwer123123123",
            TokenLifetime = 1000
        };

        _credentials = new UserCredential()
        {
            Id = Guid.NewGuid(),
            Details = new UserDetails()
            {
                Email = "test@test.com"
            }
        };
    }

    [Test]
    public void Generate_CredentialsIsValid_ShouldReturnHash()
    {
        // Arrange
        var jwtTokenService = new JwtTokenService();

        // Act 
        var result = jwtTokenService.Generate(_authOptions, _credentials);

        // Assert
        result.Should().NotBeNullOrEmpty();
    }
}