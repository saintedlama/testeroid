
using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace Testeroid
{
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