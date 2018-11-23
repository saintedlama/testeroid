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

        public void Generate(CoverageResult coverageResult)
        {
            foreach (var report in _reports)
            {
                report.Generate(coverageResult);
            }
        }
    }
}