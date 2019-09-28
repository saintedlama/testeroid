# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/saintedlama/versionize) for commit guidelines.

<a name="1.6.0"></a>
## 1.6.0 (2019-9-28)

### Bug Fixes

* switch to reading stdout and stderr when building the execution result
* fix stdout and stderr redirection

### Features

* updated coverlet to v5.2.0 tag

## 1.5.0 (2019-3-21)

### Features

* update coverlet

## 1.4.0 (2019-3-13)

### Features

* update coverlet

## 1.3.0 (2019-3-12)

### Features

* increase timeout to 120 sec and kill processes that do not terminate within timeout and exit with non zero exit code

## 1.2.0 (2019-2-27)

### Features

* add a timeout option when executing processes

## 1.1.2 (2018-12-17)

### Bug Fixes

* escape directory in dotnet test to avoid errors with directories with whitespaces
* throw origin exception

## 1.1.1 (2018-11-29)

### Bug Fixes

* remove duplicate file deleting logic
* better error message if no targetFramework could be derived from tests
* print nicer help for unknown arguments and delete intermediate report files

## 1.1.0 (2018-11-26)

### Features

* emit html reports
* WIP add report generator prototype html report

## 1.0.1 (2018-11-26)

## 1.0.0 (2018-11-26)

### Features

* add opencover report

