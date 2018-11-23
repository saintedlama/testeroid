using System;
using System.Drawing;

namespace Testeroid.CommandLine
{

    public interface IPlatformAbstractions
    {
        void Exit(int exitCode);
        void WriteLine(string message, ConsoleColor color);
        void Write(string message, ConsoleColor color);
    }
}
