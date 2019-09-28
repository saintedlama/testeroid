using System;
using System.IO;
using ConsoleTables;
using Coverlet.Core;
using Coverlet.Core.Reporters;
using static Testeroid.CommandLine.CommandLineUI;

namespace Testeroid.Reports
{
    public class ConsoleSummaryReport : IReport
    {
        public void Generate(ReportContext reportContext)
        {
            var summary = new CoverageSummary();
            var coverageTable = new ConsoleTable("Module", "Line", "Branch", "Method");

            var modules = reportContext.CoverageResult.Modules;

            foreach (var module in modules)
            {
                var linePercent = summary.CalculateLineCoverage(module.Value).Percent ;
                var branchPercent = summary.CalculateBranchCoverage(module.Value).Percent;
                var methodPercent = summary.CalculateMethodCoverage(module.Value).Percent;

                coverageTable.AddRow(Path.GetFileNameWithoutExtension(module.Key), $"{linePercent}%", $"{branchPercent}%", $"{methodPercent}%");
            }

            var overallLineCoverage = summary.CalculateLineCoverage(modules).Percent;
            var overallBranchCoverage = summary.CalculateBranchCoverage(modules).Percent;
            var overallMethodCoverage = summary.CalculateMethodCoverage(modules).Percent;

            Information();
            Information(coverageTable.ToMinimalString());

            Information($"Total Line: {overallLineCoverage}%");
            Information($"Total Branch: {overallBranchCoverage}%");
            Information($"Total Method: {overallMethodCoverage}%");

            reportContext.AddReport(new Report { Format = "console" });
        }
    }
}