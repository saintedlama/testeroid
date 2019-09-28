using System;
using System.Diagnostics;

namespace Testeroid
{
    public static class StringExtensions
    {
        public static Execution Execute(this string cmd, string arguments = null, string workingDirectory = null, int timeoutMilliseconds = 2 * 60 * 1000)
        {
            var sw = new Stopwatch();
            sw.Start();

            var process = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = workingDirectory,
                    FileName = cmd,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    
                }
            };

            process.Start();

            var hasExited = process.WaitForExit(timeoutMilliseconds);

            if (!hasExited)
            {
                try
                {
                    process.Kill();
                }
                catch (Exception ex)
                {
                    throw new ProcessTerminationException($"Could not kill the non exiting process due to exception", ex);
                }

                throw new ProcessTerminationException($"The process did not exit within the defined timeout of {timeoutMilliseconds}ms");
            }

            sw.Stop();

            return new Execution
            {
                ExitCode = process.ExitCode,
                StandardOutput = process.StandardOutput.ReadToEnd(),
                StandardError = process.StandardError.ReadToEnd(),
                ElapsedMilliseconds = sw.ElapsedMilliseconds,
            };
        }
    }

    public class ProcessTerminationException : Exception
    {
        public ProcessTerminationException(string message) : base(message) { }
        public ProcessTerminationException(string message, Exception ex) : base(message, ex) { }
    }

    public class Execution
    {
        public int ExitCode { get; set; }
        public string StandardOutput { get; set; }
        public string StandardError { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}