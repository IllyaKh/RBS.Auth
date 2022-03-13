using System.Text.RegularExpressions;

namespace RBS.Auth.WebApi.Validators.Rules;

public static class PasswordRules
{
    public static bool HasNeededNumberOfSymbols(string password)
    {
        var hasMinimum8Chars = new Regex(@".{8,25}");

        return hasMinimum8Chars.IsMatch(password);
    }

    public static bool HasSpecialSymbols(string password)
    {
        var hasSpecialSymbol = new Regex(@".*[*.!@$%^&(){}[]:;<>,.?/~_+-=|\]");

        return hasSpecialSymbol.IsMatch(password);
    }

    public static bool PasswordHasUppercase(string password)
    {
        var hasUpperCase = new Regex(@"[A-Z]+");

        return hasUpperCase.IsMatch(password);
    }

    public static bool HasNumber(string password)
    {
        var hasNumber = new Regex(@"[0-9]+");

        return hasNumber.IsMatch(password);
    }
}