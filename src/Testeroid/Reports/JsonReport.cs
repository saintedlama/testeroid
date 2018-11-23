using System.IO;
using Coverlet.Core;
using Coverlet.Core.Reporters;

namespace Testeroid.Reports
{
    public class JsonReport : IReport
    {
        private readonly string _outputPath;

        public JsonReport(string outputPath)
        {
            _outputPath = outputPath;
        }

        public void Generate(CoverageResult coverageResult)
        {
            var reporter = new ReporterFactory("json").CreateReporter();

            var outputFile = Path.Combine(_outputPath, "coverage.json");

            File.WriteAllText(outputFile, reporter.Report(coverageResult));
        }
    }
}