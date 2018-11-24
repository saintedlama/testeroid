using System;
using McMaster.Extensions.CommandLineUtils;

using Coverlet.Core;
using Coverlet.Core.Reporters;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using ConsoleTables;
using System.Xml;
using System.Linq;
using Testeroid.CommandLine;
using static Testeroid.CommandLine.CommandLineUI;
using Testeroid.Reports;

namespace Testeroid
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "testeroid";
            app.FullName = "dotnet test on steroids";
            app.HelpOption("-h|--help");
            app.VersionOption("-v|--version", GetAssemblyVersion());

            var buildConfigurationOption = app.Option("-c|--configuration <CONFIGURATION>",
                "configuration to use for building the project. The default is 'Debug'.", CommandOptionType.SingleValue);

            var noRestoreOption = app.Option("--no-restore", "Do not restore the project before building.", CommandOptionType.NoValue);
            var noBuildOption = app.Option("--no-build", "Do not build the project.", CommandOptionType.NoValue);

            var workingDirectoryOption = app.Option("-d|--directory <DIRECTORY>",
                "Specifies the Working directory. If specified a solution or a csproj file is discovered using discovery logic in the working directory.", CommandOptionType.SingleValue);

            var silentOption = app.Option("--silent", "Do not write to standard output.", CommandOptionType.NoValue);
            var verboseOption = app.Option("--verbose", "Write verbose information to standard output.", CommandOptionType.NoValue);

            var reportOption = app.Option("--report <REPORT>", "Specify which reports to create: console, cobertura, opencover or lcov.By default console, cobertura and lcov are created", CommandOptionType.MultipleValue);
            
            var testLoggerOption = app.Option("--test-logger <LOGGER>", "Specify which logger should be used for 'dotnet test'.", CommandOptionType.MultipleValue);

            var excludeOption = app.Option("--exclude <PATTERN>", "Excludes assembly or types using '[Assembly-Filter]Type-Filter' syntax with wildcards '*' and '?'. For example: --exclude '[*]Testeroid.*' will exclude all types in Testeroid namespace in any assembly.", CommandOptionType.MultipleValue);
            var includeOption = app.Option("--include <PATTERN>", "Includes assembly or types using '[Assembly-Filter]Type-Filter' syntax with wildcards '*' and '?'. For example: --include '[*]Testeroid.*' will include all types in Testeroid namespace in any assembly.", CommandOptionType.MultipleValue);

            var excludePathOption = app.Option("--exclude-path <PATTERN>", "Excludes directories from test project discovery. To exclude a directory 'fixtures' use --exclude-path fixtures. Note all directories with name containing fixture will be excluded.", CommandOptionType.MultipleValue);

            app.OnExecute(() =>
            {
                if (silentOption.HasValue())
                {
                    CommandLineUI.Verbosity = VerbosityLevel.Silent;
                }

                if (verboseOption.HasValue())
                {
                    CommandLineUI.Verbosity = VerbosityLevel.Verbose;
                }

                var buildConfiguration = buildConfigurationOption.Value() ?? "Debug";
                var reports = reportOption.HasValue() ? reportOption.Values : new List<string>() { 
                    "opencover",
                    "cobertura",
                    "lcov",
                    "html",
                    "console", 
                };

                var excludes = excludeOption.Values.ToArray();
                var includes = includeOption.Values.ToArray();
                
                var testLogger = testLoggerOption.HasValue()?$"--logger {testLoggerOption.Value()}":String.Empty;

                // Discover test projects
                var workingDirectory = WorkingDirectory.Discover(workingDirectoryOption.Value() ?? Directory.GetCurrentDirectory(), excludePathOption.Values);

                if (workingDirectory == null)
                {
                    Exit("Could not find a solution or project in the current directory or any of it's parents", 1);
                }

                Information($"  Using {workingDirectory.Path.FullName} as working directory");
                Information($"  Discovered {workingDirectory.TestProjects.Count()} test projects...");

                if (!noBuildOption.HasValue())
                {
                    var build = "dotnet".Execute($"build --configuration {buildConfiguration} {(noRestoreOption.HasValue() ? "--no-restore" : "")}",
                        workingDirectory: workingDirectory.Path.FullName);

                    if (build.ExitCode != 0)
                    {
                        Step(StepResult.Failed, $"Failed to build {workingDirectory.Path.FullName}");
                        Exit(build.StandardOutput, 1);
                    }
                    else
                    {
                        Step(StepResult.Passed, $"Build finished ({build.ElapsedMilliseconds}ms)");
                        Verbose(build.StandardOutput);
                    }
                }

                // Find out where these projects build to
                string lastCoverageReport = null;

                var reportOutputPath = Path.Combine(workingDirectory.Path.FullName, "coverage");

                if (!Directory.Exists(reportOutputPath))
                {
                    Directory.CreateDirectory(reportOutputPath);
                }

                var resultingReports = BuildResultReportsPipeline(reportOutputPath, reports);
                var exitCode = 0;

                for (var i = 0; i < workingDirectory.TestProjects.Count; i++)
                {
                    var project = workingDirectory.TestProjects[i];

                    var testDll = project.GetDllPath(buildConfiguration);

                    if (!File.Exists(testDll))
                    {
                        Exit($"Expected the project {project.ProjectFile.FullName} to build a test dll to {testDll} but this file does not exits", 1);
                    }

                    Coverage coverage = new Coverage(testDll, excludes, includes, new string[0], lastCoverageReport);
                    coverage.PrepareModules();

                    var dotnetTest = "dotnet".Execute($"test {project.GetDirectory()} --no-build --no-restore {testLogger}", workingDirectory: workingDirectory.Path.FullName);

                    Verbose(dotnetTest.StandardOutput);

                    if (dotnetTest.ExitCode != 0)
                    {
                        Step(StepResult.Failed, $"Test execution failed for {project.ProjectFile.FullName}");
                        Information(dotnetTest.StandardError);

                        exitCode = 1;
                    }
                    else
                    {
                        Step(StepResult.Passed, $"Tested {workingDirectory.RelativePath(project.ProjectFile)} ({dotnetTest.ElapsedMilliseconds}ms)");
                    }

                    var coverageResult = coverage.GetCoverageResult();

                    if (lastCoverageReport != null && File.Exists(lastCoverageReport))
                    {
                        try
                        {
                            File.Delete(lastCoverageReport);
                        }
                        catch (Exception ex)
                        {
                            Verbose($"Could not delete intermediate coverage report {lastCoverageReport} due to error {ex.Message}");
                            Verbose($"{ex.StackTrace.ToString()}");
                        }
                    }

                    if (i == workingDirectory.TestProjects.Count - 1)
                    {
                        Information($"  Saving reports to {reportOutputPath}");

                        resultingReports.Generate(coverageResult);
                    }
                    else
                    {
                        lastCoverageReport = BuildIntermediateCoverletReport(coverageResult);
                    }
                }

                return exitCode;
            });

            return app.Execute(args);
        }

        private static IReport BuildResultReportsPipeline(string outputPath, List<string> reports)
        {
            return new ReportPipeline(
                ShouldEmitReport(reports, "opencover") ? new OpenCoverReport(outputPath) : null,
                ShouldEmitReport(reports, "cobertura") ? new CoberturaReport(outputPath) : null,
                ShouldEmitReport(reports, "lcov") ? new LcovReport(outputPath) : null,
                ShouldEmitReport(reports, "html") ? new HtmlReport(outputPath) : null,
                ShouldEmitReport(reports, "console") ? new ConsoleSummaryReport() : null
            );
        }

        private static bool ShouldEmitReport(List<string> reports, string report)
        {
            return reports.Any(r => r.Equals(report, StringComparison.InvariantCultureIgnoreCase));
        }

        private static void BuildHtmlReport(string outputDirectory, CoverageResult result)
        {
            var report = new HtmlReport(outputDirectory);
            report.Generate(result);
        }

        private static void PrintConsoleReport(CoverageResult result)
        {
            var summary = new CoverageSummary();
            var coverageTable = new ConsoleTable("Module", "Line", "Branch", "Method");

            foreach (var module in result.Modules)
            {
                var linePercent = summary.CalculateLineCoverage(module.Value).Percent * 100;
                var branchPercent = summary.CalculateBranchCoverage(module.Value).Percent * 100;
                var methodPercent = summary.CalculateMethodCoverage(module.Value).Percent * 100;

                coverageTable.AddRow(Path.GetFileNameWithoutExtension(module.Key), $"{linePercent}%", $"{branchPercent}%", $"{methodPercent}%");
            }

            var overallLineCoverage = summary.CalculateLineCoverage(result.Modules).Percent * 100;
            var overallBranchCoverage = summary.CalculateBranchCoverage(result.Modules).Percent * 100;
            var overallMethodCoverage = summary.CalculateMethodCoverage(result.Modules).Percent * 100;

            Information(coverageTable.ToMinimalString());

            Information($"Total Line: {overallLineCoverage}%");
            Information($"Total Branch: {overallBranchCoverage}%");
            Information($"Total Method: {overallMethodCoverage}%");
        }

        private static string BuildIntermediateCoverletReport(CoverageResult result)
        {
            var outputFile = Path.GetTempFileName() + ".json";

            var reporter = new ReporterFactory("json").CreateReporter();

            File.WriteAllText(outputFile, reporter.Report(result));

            return outputFile;
        }

        static string GetAssemblyVersion() => typeof(Program).Assembly.GetName().Version.ToString();
    }
}
