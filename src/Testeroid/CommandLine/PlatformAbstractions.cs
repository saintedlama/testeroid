using System;
using System.Drawing;

namespace Testeroid.CommandLine
{
    public class PlatformAbstractions : IPlatformAbstractions
    {
        public void Exit(int exitCode)
        {
            Environment.Exit(exitCode);
        }

        public void Write(string message, ConsoleColor color)
        {
            var oldColor = Console.ForegroundColor;

            Console.ForegroundColor = color;
            Console.Write(message);

            Console.ForegroundColor = oldColor;
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteLine(string message, ConsoleColor color)
        {
            var oldColor = Console.ForegroundColor;

            Console.ForegroundColor = color;
            Console.WriteLine(message);

            Console.ForegroundColor = oldColor;
        }
    }
}
