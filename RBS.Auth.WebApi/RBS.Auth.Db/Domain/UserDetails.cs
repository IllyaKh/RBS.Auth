using System;

namespace RBS.Auth.Db.Domain;

public class UserDetails
{
    public int Id { get; set; }

    public DateTime RegisteredAt { get; set; } = DateTime.Now;

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }
}