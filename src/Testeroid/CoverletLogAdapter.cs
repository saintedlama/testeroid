using System;
using Coverlet.Core.Logging;
using static Testeroid.CommandLine.CommandLineUI;

namespace Testeroid
{
    public class CoverletLogAdapter : ILogger
    {
        // TODO: Decide what to do with coverlet logs
        public void LogError(string message)
        {
            Log("Error", message);
        }

        public void LogError(Exception exception)
        {
            Log("Error", exception.Message + exception.StackTrace);
        }

        public void LogInformation(string message)
        {
            Log("Info", message);
        }

        public void LogVerbose(string message)
        {
            Log("Verbose", message);
        }

        public void LogWarning(string message)
        {
            Log("Warning", message);
        }

        private void Log(string level, string message)
        {
            Verbose($"[{level}] Coverlet: {message}");
        }
    }
}