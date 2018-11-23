using System.IO;
using Coverlet.Core;
using Coverlet.Core.Reporters;

namespace Testeroid.Reports
{
    public class LcovReport : IReport
    {
        private readonly string _outputPath;

        public LcovReport(string outputPath)
        {
            _outputPath = outputPath;
        }

        public void Generate(CoverageResult coverageResult)
        {
            var reporter = new ReporterFactory("lcov").CreateReporter();

            var outputFile = Path.Combine(_outputPath, "coverage.lcov");

            File.WriteAllText(outputFile, reporter.Report(coverageResult));
        }
    }
}