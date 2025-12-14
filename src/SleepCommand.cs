// Licensed under the MIT License
// https://github.com/sator-imaging/GitHubWorkflow

using System;
using System.Threading;

namespace GitHubWorkflow;

internal static class SleepCommand
{
    public static void Run(double seconds)
    {
        var duration = ValidateSeconds(seconds);
        Thread.Sleep(duration);
    }

    public static TimeSpan ValidateSeconds(double seconds)
    {
        if (double.IsNaN(seconds) || double.IsInfinity(seconds) || seconds <= 0)
        {
            throw new InvalidOperationException("Sleep duration must be a positive number.");
        }

        var milliseconds = seconds * 1000;
        if (milliseconds > int.MaxValue)
        {
            throw new InvalidOperationException("Sleep duration is too long.");
        }

        return TimeSpan.FromMilliseconds(milliseconds);
    }
}
