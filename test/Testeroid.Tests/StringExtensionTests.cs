using System;
using System.Runtime.InteropServices;
using Shouldly;
using Xunit;

namespace Testeroid.Tests
{
    public class StringExtensionTests
    {
        [Fact]
        public void ShouldExecuteAProgram()
        {
            var cmd = Environment.OSVersion.Platform == PlatformID.Unix ? "ls" : "dir";

            var execution = cmd.Execute();
            execution.ExitCode.ShouldBe(0);
            execution.StandardError.ShouldBeNullOrWhiteSpace();
            execution.StandardOutput.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ShouldReturnExitCodeForCommandExitingWithNonZeroExitCode()
        {
            var execution = "dotnet".Execute("test doesnotexist");
            execution.ExitCode.ShouldNotBe(0);
        }
    }
}
