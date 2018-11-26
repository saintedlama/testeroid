using System.Collections.Generic;
using System.Linq;
using Coverlet.Core;

namespace Testeroid.Reports
{
    public class ReportPipeline : IReport
    {
        private readonly List<IReport> _reports;

        public ReportPipeline(params IReport[] reports)
        {
            _reports = reports
                .Where(r => r != null)
                .ToList();
        }

        public void Generate(ReportContext reportContext)
        {
            foreach (var report in _reports)
            {
                report.Generate(reportContext);
            }
        }
    }

    public class ReportContext
    {
        public CoverageResult CoverageResult { get; set; }
        public List<Report> Reports { get; set; } = new List<Report>();

        public Report GetReport(string format)
        {
            return Reports.FirstOrDefault(report => report.Format.Equals(format));
        }

        public void AddReport(Report report)
        {
            Reports.Add(report);
        }
    }

    public class Report
    {
        public string Path { get; set; }
        public string Format { get; set; }
    }
}