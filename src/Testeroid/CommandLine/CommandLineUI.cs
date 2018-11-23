using System;
using System.Drawing;

namespace Testeroid.CommandLine
{
    public static class CommandLineUI
    {
        public static IPlatformAbstractions Platform { get; set; } = new PlatformAbstractions();
        public static VerbosityLevel Verbosity { get; set; } = VerbosityLevel.All;

        public static void Exit(string message, int code)
        {
            if (Verbosity != VerbosityLevel.Silent)
            {
                Platform.WriteLine(message, ConsoleColor.Red);
            }

            Platform.Exit(code);
        }

        public static void Information(string message = null)
        {
            if (Verbosity == VerbosityLevel.Silent)
            {
                return;
            }

            Platform.WriteLine(message, ConsoleColor.DarkGray);
        }

        public static void Verbose(string message)
        {
            if (Verbosity == VerbosityLevel.Verbose)
            {
                Platform.WriteLine(message, ConsoleColor.Gray);
            }
        }

        public static void Step(StepResult result, string message)
        {
            if (Verbosity == VerbosityLevel.Silent)
            {
                return;
            }

            if (result == StepResult.Passed)
            {
                Platform.Write($"✓ ", ConsoleColor.Green);
            }
            else if (result == StepResult.Failed)
            {
                Platform.Write("- ", ConsoleColor.Red);
            }

            Platform.WriteLine(message, ConsoleColor.Gray);
        }
    }

    public enum StepResult
    {
        Passed,
        Failed
    }
}
