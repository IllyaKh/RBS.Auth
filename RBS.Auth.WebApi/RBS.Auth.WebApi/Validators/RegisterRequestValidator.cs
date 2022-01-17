using FluentValidation;
using RBS.Auth.WebApi.Models;
using RBS.Auth.WebApi.Validators.Rules;

namespace RBS.Auth.WebApi.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.FirstName)
            .Must(NameRules.MatchesNamePattern)
            .WithMessage("First name is invalid.");

        RuleFor(r => r.LastName)
            .Must(NameRules.MatchesNamePattern)
            .WithMessage("LastName is invalid.");

        RuleFor(r => r.Email).EmailAddress();

        RuleFor(r => r.Password)
            .Must(PasswordRules.HasNeededNumberOfSymbols)
            .WithMessage("Password should contain 8 - 25 characters.");

        RuleFor(r => r.Password)
            .Must(PasswordRules.HasNumber)
            .WithMessage("Password should at least one number.");

        RuleFor(r => r.Password)
            .Must(PasswordRules.HasSpecialSymbols)
            .WithMessage("Password should at least one special symbol.");

        RuleFor(r => r.Password)
            .Must(PasswordRules.PasswordHasUppercase)
            .WithMessage("Password should contain at least one uppercase character.");
    }   
}