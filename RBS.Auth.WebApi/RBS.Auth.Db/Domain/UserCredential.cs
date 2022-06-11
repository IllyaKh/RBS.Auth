using System;
using System.Collections.Generic;

namespace RBS.Auth.Db.Domain;

public class UserCredential
{
    public Guid Id { get; set; }

    public string Hash { get; set; }

    public string Salt { get; set; }

    public UserDetails Details { get; set; }

    public IList<UserClaim> Claims { get; set; }

    public bool IsVerified { get; set; }
}