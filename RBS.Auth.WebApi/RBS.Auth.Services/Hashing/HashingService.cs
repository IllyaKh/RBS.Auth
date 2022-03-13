using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using RBS.Auth.Services.Hashing.Options;
using RBS.Auth.Services.Interfaces.Hashing;

namespace RBS.Auth.Services.Hashing;

public sealed class HashingService : IHashingService
{
    private const int SaltSize = 16;
    private const int KeySize = 32;

    public HashingService(IOptions<HashingOptions> options)
    {
        Options = options.Value;
    }

    private HashingOptions Options { get; }

    public (string salt, string key) Hash(string password)
    {
        using var hAlg = new Rfc2898DeriveBytes(
            password,
            SaltSize,
            Options.Iterations,
            HashAlgorithmName.SHA512);

        var key = Convert.ToBase64String(hAlg.GetBytes(KeySize));
        var salt = Convert.ToBase64String(hAlg.Salt);

        return (salt, key);
    }

    public bool Check(string key, string salt, string password)
    {
        using var algorithm = new Rfc2898DeriveBytes(
            password,
            Convert.FromBase64String(salt),
            Options.Iterations,
            HashAlgorithmName.SHA512);

        var keyToCheck = algorithm.GetBytes(KeySize);

        var verified = keyToCheck.SequenceEqual(
            Convert.FromBase64String(key));

        return verified;
    }
}