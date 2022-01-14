using RBS.Auth.Db.Domain.Enums;

namespace RBS.Auth.Db.Domain;

public class UserClaim
{
    public int Id { get; set; }

    public RoleClaim Role { get; set; }

    public string Name { get; set; }
}