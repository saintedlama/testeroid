using System;
using System.IO;
using System.Runtime.InteropServices;
using Shouldly;
using Xunit;

namespace Testeroid.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void ShouldGenerateCoberaturaOpencoverAndLcovReports()
        {
            var fixturesDirectory = LocateFixturesDirectory(Directory.GetCurrentDirectory());

            var exitCode = Program.Main(new string[] { "--directory", fixturesDirectory });

            var coverageReportsDirectory = Path.Join(fixturesDirectory, "coverage");

            Directory.Exists(coverageReportsDirectory).ShouldBeTrue();
            
            var reportFiles = Directory.GetFiles(coverageReportsDirectory);

            reportFiles.ShouldContain(Path.Join(coverageReportsDirectory, "coverage.cobertura.xml"));
            reportFiles.ShouldContain(Path.Join(coverageReportsDirectory, "coverage.opencover.xml"));
            reportFiles.ShouldContain(Path.Join(coverageReportsDirectory, "coverage.lcov"));
        }

        private string LocateFixturesDirectory(string directory)
        {
            var fixturesDirectory = Path.Join(directory, "fixtures");

            if (Directory.Exists(fixturesDirectory))
            {
                return fixturesDirectory;
            }

            return LocateFixturesDirectory(Directory.GetParent(directory).FullName);
        }
    }
}