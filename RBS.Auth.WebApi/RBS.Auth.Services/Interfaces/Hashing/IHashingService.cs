namespace RBS.Auth.Services.Interfaces.Hashing;

public interface IHashingService
{
    (string salt, string key) Hash(string password);

    bool Check(string key, string salt, string password);
}