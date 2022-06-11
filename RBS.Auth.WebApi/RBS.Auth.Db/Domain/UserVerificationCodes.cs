using System;

namespace RBS.Auth.Db.Domain
{
    public class UserVerificationCodes
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public DateTime ExpireDate { get; set; }

        public UserCredential User { get; set; }

        public Guid UserId { get; set; }

    }
}
