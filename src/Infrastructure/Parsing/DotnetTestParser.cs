using System.Text.RegularExpressions;

namespace Infrastructure.Parsing;

public static class DotnetTestParser
{
    public static string? ExtractFailure(string logs)
    {
        var testName = Regex.Match(
            logs,
            @"\[\s*FAIL\s*\]\s*(.+)"
        );

        var message = Regex.Match(
            logs,
            @"Assert\..+Failure"
        );

        var location = Regex.Match(
            logs,
            @"([A-Za-z0-9_\.]+\.cs\(\d+,\d+\))"
        );

        if (!testName.Success)
            return null;

        return $"""
Failure reproduced:

Test: {testName.Groups[1].Value}
Message: {message.Value}
Location: {location.Value}
""";
    }
}