using System;
using System.IO;
using System.Runtime.InteropServices;
using Shouldly;
using Xunit;

namespace Testeroid.Tests
{
    public class WorkingDirectoryTests
    {
        private readonly DirectoryInfo _projectRootDirectory;
        private readonly string _fixturesDirectory;

        public WorkingDirectoryTests()
        {
            var cwd = new DirectoryInfo(Directory.GetCurrentDirectory());

            _projectRootDirectory = cwd.Parent.Parent.Parent.Parent.Parent;
            _fixturesDirectory =  Path.Combine(_projectRootDirectory.FullName, "fixtures");
        }

        [Fact]
        public void ShouldUseProjectWhenDiscoveringTestProjects()
        {
            var fixtureProjectDirectory = Path.Join(_fixturesDirectory, "Fixture.Tests");
            var workingDirectory = WorkingDirectory.Discover(fixtureProjectDirectory);

            Assert.Equal(fixtureProjectDirectory, workingDirectory.Path.FullName);
        }

        
        [Fact]
        public void ShouldUseSolutionWhenDiscoveringTestProjectsButTheProjectIsNotTestable()
        {
            var fixtureProjectDirectory = Path.Join(_fixturesDirectory, "Fixture");
            var workingDirectory = WorkingDirectory.Discover(fixtureProjectDirectory);

            Assert.Equal(_fixturesDirectory, workingDirectory.Path.FullName);
        }
    }
}