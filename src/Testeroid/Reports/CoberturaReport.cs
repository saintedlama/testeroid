using System.IO;
using Coverlet.Core;
using Coverlet.Core.Reporters;

namespace Testeroid.Reports
{
    public class CoberturaReport : IReport
    {
        private readonly string _outputPath;

        public CoberturaReport(string outputPath)
        {
            _outputPath = outputPath;
        }

        public void Generate(ReportContext reportContext)
        {
            var reporter = new ReporterFactory("cobertura").CreateReporter();

            var outputFile = Path.Combine(_outputPath, "coverage.cobertura.xml");

            File.WriteAllText(outputFile, reporter.Report(reportContext.CoverageResult));

            reportContext.AddReport(new Report { Format = "cobertura", Path = outputFile });
        }
    }
}