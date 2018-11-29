# Testeroid

[![Travis build status](https://travis-ci.org/saintedlama/testeroid.svg?branch=master)](https://travis-ci.org/saintedlama/testeroid)
[![Appveyor build status](https://ci.appveyor.com/api/projects/status/oijtj75bopgxgudn?svg=true)](https://ci.appveyor.com/project/saintedlama/testeroid)
[![Coverage Status](https://coveralls.io/repos/saintedlama/testeroid/badge.svg?branch=)](https://coveralls.io/r/saintedlama/testeroid?branch=master)
[![Conventional Commits](https://img.shields.io/badge/Conventional%20Commits-1.0.0-yellow.svg)](https://conventionalcommits.org)

> dotnet test on steroids

_how it works:_

1. Discovers testable projects
    * **Auto discovery**: Include a reference to `Microsoft.NET.Test.Sdk` in the csproj file
    * **Override**: Mark your project with `<IsTestable>true</IsTestable>` in the csproj file to include/exclude the test project
2. Executes tests with code coverage using the awesome [coverlet](https://github.com/tonerdo/coverlet)
3. Merges results of all test projects into one coverage result
4. Generates reports
    * lcov
    * cobertura
    * console summary
    * ~~html~~ (in progress)

## Installation

```bash
dotnet tool install --global Testeroid
```

## Usage

```bash
Usage: testeroid [options]

Options:
  -h|--help                           Show help information
  -v|--version                        Show version information
  -c|--configuration <CONFIGURATION>  configuration to use for building the project. The default is 'Debug'.
  --no-restore                        Do not restore the project before building.
  --no-build                          Do not build the project.
  -d|--directory <DIRECTORY>          Specifies the Working directory. If specified a solution or a csproj file is discovered using discovery logic in the working directory.
  --silent                            Do not write to standard output.
  --verbose                           Write verbose information to standard output.
  --report <REPORT>                   Specify which reports to create: console, cobertura or lcov.By default console, cobertura and lcov are created
  --test-logger <LOGGER>              Specify which logger should be used for 'dotnet test'.
  --exclude <PATTERN>                 Excludes assembly or types using '[Assembly-Filter]Type-Filter' syntax with wildcards '*' and '?'. For example: --exclude '[*]Testeroid.*' will exclude all types in Testeroid namespace in any assembly.
  --include <PATTERN>                 Includes assembly or types using '[Assembly-Filter]Type-Filter' syntax with wildcards '*' and '?'. For example: --include '[*]Testeroid.*' will include all types in Testeroid namespace in any assembly.
  --exclude-path <PATTERN>            Excludes directories from test project discovery. To exclude a directory 'fixtures' use --exclude-path fixtures. Note all directories with name containing fixture will be excluded.
```

Testeroid recognize test projects by testing if they contain a `<IsTestable>true</IsTestable&>` or include a reference to `Microsoft.NET.Test.Sdk` element in their `csproj` project file.

## Roadmap

* [x] HTML Reports

## Kudos

Most of the work that is done by `testeroid` is based or done by [coverlet](https://github.com/tonerdo/coverlet).

HTML Reports generated with [Report Generator](https://github.com/danielpalme/ReportGenerator).