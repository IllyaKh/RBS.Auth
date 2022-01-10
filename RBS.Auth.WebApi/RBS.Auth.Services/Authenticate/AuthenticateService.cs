using RBS.Auth.Common.Enums;
using RBS.Auth.Common.Models;
using RBS.Auth.Services.Interfaces.Authenticate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RBS.Auth.Services.Authenticate
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly List<Account> _accs = new List<Account>()
        {
            new Account()
            {
                Id = Guid.Parse("2b18f575-784c-4ad7-ae33-abb73ab8e7b6"),
                Email = "illia.khomenko@nure.ua",
                Password = "admin",
                Roles = new Role[] { Role.Admin }
            }
        };

        public Account Authenticate(string email, string password)
        {
            return _accs.SingleOrDefault(u => u.Email == email && u.Password == password);
        }
    }
}
