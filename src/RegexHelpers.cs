using System.Text.RegularExpressions;

namespace GitHubWorkflow;

internal static partial class RegexHelpers
{
    [GeneratedRegex(@"\$\{\{\s*(matrix|inputs)\s*\.\s*([A-Za-z0-9_-]+)\s*\}\}")]
    public static partial Regex PlaceholderPattern { get; }

    [GeneratedRegex(@"\s*>>?\s*\$GITHUB_STEP_SUMMARY")]
    public static partial Regex StepSummaryRedirectPattern { get; }

    [GeneratedRegex(@"#.*$")]
    public static partial Regex InlineCommentPattern { get; }

    [GeneratedRegex(@"\$([0-9])")]
    public static partial Regex DollarPositionalPattern { get; }

    [GeneratedRegex(@"^\s*sleep\s+([0-9]+)\s*;?\s*$", RegexOptions.Multiline)]
    public static partial Regex SleepCommandPattern { get; }
}
