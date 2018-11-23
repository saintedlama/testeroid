
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

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

    public class TestProject
    {
        public FileInfo ProjectFile { get; }

        public TestProject(FileInfo projectFile)
        {
            ProjectFile = projectFile;
        }

        public string GetDirectory()
        {
            return ProjectFile.DirectoryName;
        }

        public string GetBuildDirectory(string buildConfiguration)
        {
            return Path.Combine(GetDirectory(), "bin", buildConfiguration, GetTargetFrameworks().First());
        }

        public string GetDllPath(string buildConfiguration)
        {
            var dllFileName = Path.GetFileNameWithoutExtension(ProjectFile.Name) + ".dll";
            return Path.Combine(GetBuildDirectory(buildConfiguration), dllFileName);
        }

        public string[] GetTargetFrameworks()
        {
            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;

            try
            {
                doc.Load(ProjectFile.FullName);
            }
            catch (Exception)
            {
                return null;
            }

            var targetFramework = doc.SelectSingleNode("/Project/PropertyGroup/TargetFramework")?.InnerText;

            if (String.IsNullOrWhiteSpace(targetFramework))
            {
                return null;
            }

            return targetFramework.Split(';');
        }

        public bool IsTestable()
        {
            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;

            try
            {
                doc.Load(ProjectFile.FullName);
            }
            catch (Exception)
            {
                return false;
            }

            var isTestableValue = doc.SelectSingleNode("/Project/PropertyGroup/IsTestable")?.InnerText;

            if (!String.IsNullOrWhiteSpace(isTestableValue))
            {
                try
                {
                    return Boolean.Parse(isTestableValue);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            // Check if it's using the Microsoft.NET.Test.Sdk
            foreach (XmlNode packageReference in doc.SelectNodes("/Project/ItemGroup/PackageReference"))
            {
                var packageReferenceInclude = packageReference?.Attributes?["Include"]?.Value;

                if ("Microsoft.NET.Test.Sdk".Equals(packageReferenceInclude, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}