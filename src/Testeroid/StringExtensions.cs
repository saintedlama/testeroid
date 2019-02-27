using System.Diagnostics;

namespace Testeroid
{
    public static class StringExtensions
    {
        public static Execution Execute(this string cmd, string arguments = null, string workingDirectory = null, int timeoutMillisecods = 50000)
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

            process.Start();

            var standardOutput = process.StandardOutput.ReadToEnd();
            var standardError = process.StandardError.ReadToEnd();

            process.WaitForExit(timeoutMillisecods);

            sw.Stop();

            return new Execution
            {
                ExitCode = process.ExitCode,
                StandardError = standardError,
                StandardOutput = standardOutput,
                ElapsedMilliseconds = sw.ElapsedMilliseconds,
            };
        }
    }

    public class Execution
    {
        public int ExitCode { get; set; }
        public string StandardOutput { get; set; }
        public string StandardError { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}