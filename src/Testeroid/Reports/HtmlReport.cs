
using System;
using System.Globalization;
using System.IO;
using System.Text;
using Coverlet.Core;

namespace Testeroid.Reports
{
    // TODO: Experimental
    public class HtmlReport : IReport
    {
        private readonly string _outputPath;

        public HtmlReport(string outputPath)
        {
            _outputPath = outputPath;
        }

        public void Generate(CoverageResult coverageResult)
        {
            // TODO: Add source code
            // TODO: Build a report for every namespace/module/class
            File.WriteAllText(Path.Combine(_outputPath), BuildOverview(coverageResult));
        }

        private string BuildOverview(CoverageResult result)
        {
            // TODO: inline the stylesheet
            return $@"
                <html>
                    <head>
                         <link rel=""stylesheet"" type=""text/css"" href=""coverage.css"" />
                         <link rel=""stylesheet"" type=""text/css"" href=""dev.css"" />
                    <head>
                    <body>
                        <div class=""container"">
                            <div class=""row"">
                                <div class=""col""> 
                                    <h1>Coverage Report</h1>
                                    {GenerateModulesTable(result)}

                                    {GenerateModuleReports(result)}
                                </div>
                            </div>
                        </div>
                    </body>
                </html>
            ";
        }

        private string GenerateModulesTable(CoverageResult result)
        {
            var rows = new StringBuilder();

            var summary = new CoverageSummary();

            foreach (var module in result.Modules)
            {
                var linePercent = summary.CalculateLineCoverage(module.Value).Percent * 100;
                var branchPercent = summary.CalculateBranchCoverage(module.Value).Percent * 100;
                var methodPercent = summary.CalculateMethodCoverage(module.Value).Percent * 100;

                rows.AppendLine($@"
                <tr>
                    <td>
                        <a href=""#{GetModuleHtmlId(module.Key)}"">
                            {Path.GetFileNameWithoutExtension(module.Key)}
                        </a>
                    </td>
                    <td>{CoveragePercentage(linePercent)}</td>
                    <td>{CoveragePercentage(branchPercent)}</td>
                    <td>{CoveragePercentage(methodPercent)}</td>
                </tr>");
            }

            var overallLineCoverage = summary.CalculateLineCoverage(result.Modules).Percent * 100;
            var overallBranchCoverage = summary.CalculateBranchCoverage(result.Modules).Percent * 100;
            var overallMethodCoverage = summary.CalculateMethodCoverage(result.Modules).Percent * 100;

            rows.AppendLine($@"
                <tr>
                    <td>Summary</td>
                    <td>{CoveragePercentage(overallLineCoverage)}</td>
                    <td>{CoveragePercentage(overallBranchCoverage)}</td>
                    <td>{CoveragePercentage(overallMethodCoverage)}</td>
                </tr>");

            return $@"
                <table class=""table table-hover table-coverage"">
                    <thead>
                        <tr>
                            <th>Module</th>
                            <th>Line Coverage</th>
                            <th>Branch Coverage</th>
                            <th>Method Coverage</th>
                        </tr>
                    </thead>
                    <tbody>
                        {rows.ToString()}
                    </body>
                </table>
            ";
        }


        private string GenerateModuleReports(CoverageResult result)
        {
            var moduleReport = new StringBuilder();

            foreach (var module in result.Modules)
            {
                var moduleSection = new StringBuilder();
                moduleSection.AppendLine($@"
                    <h2 id=""{GetModuleHtmlId(module.Key)}"">
                        {Path.GetFileNameWithoutExtension(module.Key)}
                    </h2>");

                foreach (var document in module.Value)
                {
                    //var documentSection = new StringBuilder();
                    //documentSection.AppendLine($@"<h3>{document.Key}</h3>");

                    foreach (var @class in document.Value)
                    {
                        var classSection = new StringBuilder();
                        classSection.AppendLine($@"<h3>{@class.Key}</h3>");

                        var methodRows = new StringBuilder();

                        foreach (var method in @class.Value)
                        {
                            var summary = new CoverageSummary();
                            var linePercent = summary.CalculateLineCoverage(method.Value.Lines).Percent * 100;
                            var branchPercent = summary.CalculateBranchCoverage(method.Value.Branches).Percent * 100;
                            var methodPercent = summary.CalculateMethodCoverage(method.Value.Lines).Percent * 100;

                            methodRows.AppendLine($@"
                                <tr>
                                    <td>{GetMethodName(method.Key)}</td>
                                    <td>
                                        {CoveragePercentage(linePercent)}
                                    </td>
                                    <td>
                                        {CoveragePercentage(methodPercent)}
                                    </td>
                                    <td>
                                        {CoveragePercentage(branchPercent)}
                                    </td>
                                </tr>");
                        }

                        classSection.AppendLine($@"
                            <table class=""table table-hover table-coverage"">
                                <thead>
                                    </tr>
                                        <th>Method</th>
                                        <th>Line Coverage</th>
                                        <th>Branch Coverage</th>
                                        <th>Method Coverage</th>
                                    <tr>
                                </thead>
                                <tbody>
                                    {methodRows.ToString()}
                                </tbody>
                            </table>");

                        moduleSection.AppendLine(classSection.ToString());
                    }
                }

                moduleReport.AppendLine(moduleSection.ToString());
            }

            return moduleReport.ToString();
        }

        private string CoveragePercentage(double percentage)
        {
            return $@"
                <div class=""progress"">
                    <div class=""progress-bar"" role=""progressbar"" style=""width: {percentage.ToString(CultureInfo.InvariantCulture)}%;"" aria-valuenow=""25"" aria-valuemin=""0"" aria-valuemax=""100"">
                        {percentage}%
                    </div>
                </div>";
        }

        private string GetModuleHtmlId(string moduleKey)
        {
            return "module-" + Path.GetFileNameWithoutExtension(moduleKey).Replace(".", "-");
        }

        private string GetMethodName(string methodKey)
        {
            var parts = methodKey.Split(' ');
            var returnType = parts[0];

            var nameAndNamespace = parts[1].Split("::");

            var methodName = nameAndNamespace[1];

            if (methodName.StartsWith(".ctor"))
            {
                return "Constructor";
            }

            if (methodName.StartsWith("get_"))
            {
                return $@"<span>
                        <span class=""badge badge-secondary small"">Property - get</span>
                        <span>{methodName.Replace("get_", "")}</span>
                    </span>
                ";
            }

            if (methodName.StartsWith("set_"))
            {
                return $@"<span>
                        <span class=""badge badge-secondary small"">Property - set</span>
                        <span>{methodName.Replace("set_", "")}</span>
                    </span>
                ";
            }

            return methodName;
        }
    }
}