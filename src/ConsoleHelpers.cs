using System;

namespace GitHubWorkflow;

internal static class ConsoleHelpers
{
    public static int RunWithErrorHandling(Func<int> action)
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            WriteError($"error: {ex.Message}");
            return 1;
        }
    }

    private static void WriteError(string message)
    {
        var original = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        try
        {
            Console.Error.WriteLine(message);
        }
        finally
        {
            Console.ForegroundColor = original;
        }
    }
}
