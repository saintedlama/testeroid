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
        public void Generate(CoverageResult coverageResult)
        {
            var summary = new CoverageSummary();
            var coverageTable = new ConsoleTable("Module", "Line", "Branch", "Method");

            foreach (var module in coverageResult.Modules)
            {
                var linePercent = summary.CalculateLineCoverage(module.Value).Percent * 100;
                var branchPercent = summary.CalculateBranchCoverage(module.Value).Percent * 100;
                var methodPercent = summary.CalculateMethodCoverage(module.Value).Percent * 100;

                coverageTable.AddRow(Path.GetFileNameWithoutExtension(module.Key), $"{linePercent}%", $"{branchPercent}%", $"{methodPercent}%");
            }

            var overallLineCoverage = summary.CalculateLineCoverage(coverageResult.Modules).Percent * 100;
            var overallBranchCoverage = summary.CalculateBranchCoverage(coverageResult.Modules).Percent * 100;
            var overallMethodCoverage = summary.CalculateMethodCoverage(coverageResult.Modules).Percent * 100;

            Information();
            Information(coverageTable.ToMinimalString());

            Information($"Total Line: {overallLineCoverage}%");
            Information($"Total Branch: {overallBranchCoverage}%");
            Information($"Total Method: {overallMethodCoverage}%");
        }
    }
}