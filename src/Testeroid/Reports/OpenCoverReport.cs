using System.IO;
using Coverlet.Core;
using Coverlet.Core.Reporters;

namespace Testeroid.Reports
{
    public class OpenCoverReport : IReport
    {
        private readonly string _outputPath;

        public OpenCoverReport(string outputPath)
        {
            _outputPath = outputPath;
        }

        public void Generate(ReportContext reportContext)
        {
            var reporter = new ReporterFactory("opencover").CreateReporter();

            var outputFile = Path.Combine(_outputPath, "coverage.opencover.xml");

            File.WriteAllText(outputFile, reporter.Report(reportContext.CoverageResult));

            reportContext.AddReport(new Report { Format = "opencover", Path = outputFile });
        }
    }
}