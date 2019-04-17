using System;
using System.Diagnostics;
using System.Text;

namespace Testeroid
{
    public static class StringExtensions
    {
        public static Execution Execute(this string cmd, string arguments = null, string workingDirectory = null, int timeoutMillisecods = 2 * 60 * 1000)
        {
            var sw = new Stopwatch();
            sw.Start();

            Process process = new Process();
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.FileName = cmd;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            StringBuilder standardOutputBuilder = new StringBuilder();
            StringBuilder standardErrorBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, eventArgs) =>
            {
                if (!string.IsNullOrEmpty(eventArgs.Data))
                {
                    standardOutputBuilder.Append(eventArgs.Data);
                }
            };

            process.ErrorDataReceived += (sender, eventArgs) =>
            {
                if (!string.IsNullOrEmpty(eventArgs.Data))
                {
                    standardErrorBuilder.Append(eventArgs.Data);
                }
            };

            process.Start();

            var hasExited = process.WaitForExit(timeoutMillisecods);

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

                throw new ProcessTerminationException($"The process did not exit within the defined timeout of {timeoutMillisecods}ms");
            }

            sw.Stop();

            return new Execution
            {
                ExitCode = process.ExitCode,
                StandardOutput = standardOutputBuilder.ToString(),
                StandardError = standardErrorBuilder.ToString(),
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