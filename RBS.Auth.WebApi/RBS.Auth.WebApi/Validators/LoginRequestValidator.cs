using FluentValidation;
using RBS.Auth.WebApi.Models;
using RBS.Auth.WebApi.Validators.Rules;

namespace RBS.Auth.WebApi.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(r => r.Email).EmailAddress();
    }
}