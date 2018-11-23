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

        public void Generate(CoverageResult coverageResult)
        {
            var reporter = new ReporterFactory("cobertura").CreateReporter();

            var outputFile = Path.Combine(_outputPath, "coverage.cobertura.xml");

            File.WriteAllText(outputFile, reporter.Report(coverageResult));
        }
    }
}