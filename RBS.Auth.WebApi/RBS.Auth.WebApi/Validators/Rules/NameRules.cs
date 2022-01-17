using System.Text.RegularExpressions;

namespace RBS.Auth.WebApi.Validators.Rules;

public static class NameRules
{
    public static bool MatchesNamePattern(string name)
    {
        var namePattern = new Regex(@"^[\p{L}\p{M}' \.\-]+$");

        return namePattern.IsMatch(name);
    }
}