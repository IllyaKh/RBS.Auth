﻿using System.ComponentModel.DataAnnotations;

namespace RBS.Auth.WebApi.Models;

public class LoginRequest
{
    public string Email { get; set; }

    public string Password { get; set; }
}