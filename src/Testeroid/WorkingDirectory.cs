
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Testeroid
{
    public class WorkingDirectory
    {
        public List<TestProject> TestProjects { get; }
        public DirectoryInfo Path { get; }

        public WorkingDirectory(DirectoryInfo path, List<string> excludePaths)
        {
            Path = path;

            TestProjects = path
                .GetFiles("*.csproj", SearchOption.AllDirectories)
                .Where(p => !excludePaths.Any(e => p.FullName.Contains(e)))
                .Select(p => new TestProject(p))
                .Where(p => p.IsTestable())
                .ToList();
        }

        public string RelativePath(FileInfo fileInfo)
        {
            return RelativePath(fileInfo.FullName);
        }

        public string RelativePath(string path)
        {
            return path.Replace(Path.FullName, String.Empty).TrimStart(new[] { '\\', '/' });
        }

        public static WorkingDirectory Discover(string cwd, List<string> excludePaths = null)
        {
            if (excludePaths == null)
            {
                excludePaths = new List<string>();
            }

            var projectDirectory = GetDirectoryRecursive(new DirectoryInfo(cwd), "*.csproj",  m => new TestProject(m).IsTestable());

            if (projectDirectory != null)
            {
                return new WorkingDirectory(projectDirectory, excludePaths);
            }

            var solutionDirectory = GetDirectoryRecursive(new DirectoryInfo(cwd), "*.sln", m => true);

            if (solutionDirectory != null)
            {
                return new WorkingDirectory(solutionDirectory, excludePaths);
            }

            return null;
        }

        private static DirectoryInfo GetDirectoryRecursive(DirectoryInfo directory, string pattern, Predicate<FileInfo> matchFile)
        {
            if (directory == null)
            {
                return null;
            }

            if (!directory.Exists)
            {
                return null;
            }

            var matchedFile = directory.GetFiles(pattern).FirstOrDefault();

            if (matchedFile != null && (matchFile == null || matchFile(matchedFile)))
            {
                return directory;
            }

            return GetDirectoryRecursive(directory.Parent, pattern, matchFile);
        }
    }
}