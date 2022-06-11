using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RBS.Auth.Common.Models;
using RBS.Auth.Db;
using RBS.Auth.Db.Domain;
using RBS.Auth.Db.Domain.Enums;
using RBS.Auth.Services.Interfaces.Authenticate;
using RBS.Auth.Services.Interfaces.Hashing;

namespace RBS.Auth.Services.Authenticate;

public class AuthenticateService : IAuthenticateService
{
    private readonly AuthContext _db;
    private readonly IHashingService _hashingService;
    private readonly ILogger<AuthenticateService> _logger;

    public AuthenticateService(AuthContext db,
        IHashingService hashingService,
        ILogger<AuthenticateService> logger)
    {
        _db = db;
        _hashingService = hashingService;
        _logger = logger;
    }

    public async Task<UserCredential> Authenticate(string email, string password)
    {
        try
        {
            var user = await _db.UserCredentials
                .Include(det => det.Details)
                .Include(cl => cl.Claims)
                .SingleOrDefaultAsync(d => d.Details.Email == email);

            if (user != null)
                if (_hashingService.Check(user.Hash, user.Salt, password))
                    return user;

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred when login user: {email}", ex);

            return null;
        }
    }

    public async Task<UserCredential> Register(RegisterModel model)
    {
        try
        {
            var authUser = GetUserModel(model);

            await _db.AddAsync(authUser);

            return authUser;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred when register user: {model.Email}", ex);
            return null;
        }
        finally
        {
            await _db.SaveChangesAsync();
        }
    }

    private UserCredential GetUserModel(RegisterModel model)
    {
        var userDetails = new UserDetails
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email
        };

        var hashes = _hashingService.Hash(model.Password);

        var claims = new List<UserClaim>
        {
            new()
            {
                Role = RoleClaim.User,
                Name = "none"
            }
        };

        var user = new UserCredential
        {
            Id = Guid.NewGuid(),
            Hash = hashes.key,
            Salt = hashes.salt,
            Details = userDetails,
            Claims = claims
        };

        return user;
    }
}