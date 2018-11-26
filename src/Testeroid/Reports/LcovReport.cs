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

        public void Generate(ReportContext reportContext)
        {
            var reporter = new ReporterFactory("lcov").CreateReporter();

            var outputFile = Path.Combine(_outputPath, "coverage.lcov");

            File.WriteAllText(outputFile, reporter.Report(reportContext.CoverageResult));

            reportContext.AddReport(new Report { Format = "lcov", Path = outputFile });
        }
    }
}