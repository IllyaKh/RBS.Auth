﻿using RBS.Auth.Common.Enums;
using System;

namespace RBS.Auth.Common.Models
{
    public class Account
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public Role[] Roles { get; set; }
    }
}
