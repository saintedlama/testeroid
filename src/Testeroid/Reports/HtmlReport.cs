
using System;
using System.Globalization;
using System.IO;
using System.Text;
using Coverlet.Core;
using Palmmedia.ReportGenerator.Core;
using Palmmedia.ReportGenerator.Core.Logging;

namespace Testeroid.Reports
{
    public class HtmlReport : IReport
    {
        private readonly string _outputPath;

        public HtmlReport(string outputPath)
        {
            _outputPath = outputPath;
        }

        public void Generate(ReportContext reportContext)
        {
            var coberturaReport = reportContext.GetReport("cobertura");

            if (coberturaReport == null || String.IsNullOrWhiteSpace(coberturaReport.Path))
            {
                return;
            }

            var targetDirectory = Path.Combine(_outputPath, "html");

            new Generator().GenerateReport(new ReportConfiguration(
                reportFilePatterns: new string[] { coberturaReport.Path },
                targetDirectory: targetDirectory,
                historyDirectory: null,
                reportTypes: new[] { "html" },
                assemblyFilters: new string[] { },
                classFilters: new string[] { },
                fileFilters: new string[] { },
                verbosityLevel: "Off",
                tag: null
            ));

            reportContext.AddReport(new Report { Format = "html", Path = targetDirectory });
        }
    }
}