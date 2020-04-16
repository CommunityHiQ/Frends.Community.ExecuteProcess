# Frends.Community.ExecuteProcess

FRENDS Community Task for ExecuteProcessCommand

[![Actions Status](https://github.com/CommunityHiQ/Frends.Community.ExecuteProcess/workflows/PackAndPushAfterMerge/badge.svg)](https://github.com/CommunityHiQ/Frends.Community.ExecuteProcess/actions) ![MyGet](https://img.shields.io/myget/frends-community/v/Frends.Community.ExecuteProcess) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) 

- [Installing](#installing)
- [Tasks](#tasks)
     - [ExecuteProcess](#ExecuteProcess)
     - [RunProcess](#RunProcess)
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)

# Installing

You can install the task via FRENDS UI Task View or you can find the NuGet package from the following NuGet feed
https://www.myget.org/F/frends-community/api/v3/index.json and in Gallery view in MyGet https://www.myget.org/feed/frends-community/package/nuget/Frends.Community.ExecuteProcess

# Tasks

## ExecuteProcess

Executes a command with parameters. Old task method, which is kept to maintain backward compatibility.

### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| ScriptPath		| `string`	| Path to script | `cmd` or `%windir%\system32\cmd.exe` |
| Arguments			| `array<string,string>` 	| Argument name and value	| `/C`, `echo testi >> c:\test.txt` |
| WaitForResponse	| `bool`	| Wait for process response	| `true` |
| Timeout Seconds	| `int`	| Timeout for process response in full seconds.	| `10` |

### Returns

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Result        | `string`   | Command result	when waiting fo result | `External process execution was successful with output: foobar`|
| Status        | `bool`   | Status of executed command. Always true when returning a result. Returns true if command was started successfully when not waiting for a result.	| `0`|


## RunProcess

Repeats message

### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| FileName | `string` | An application or document with which to start a process. Use cmd.exe to execute command on "command line" | `/bin/bash` |
| Arguments | `array<string,string>` |  Argument name and value | `-c`,`ls` |

### Options

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| TimeoutSeconds | `int` | Timeout in full seconds.  | `30` |
| KillProcessAfterTimeout | `bool` |  true if process should be killed after timeout; otherwise, false | `false` |
| RedirectStandardInput | `bool` |  true if input should be read from StandardInput; otherwise, false | `false` |
| ThrowExceptionOnErrorResponse | `bool` | true if the task should throw exception when return code is not 0.| `false` |

### Returns

A result object with parameters


| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| ExitCode | `int` |  | `0` |
| Output | `string` | Process' STDOUT output |  `Operation complete.` |
| StdErr | `string` | Process' STDERR output |  `Error code: 20` |

Usage:
To fetch result use syntax:
`#result.ExitCode`

### Exceptions
Task could throw `TimeoutException` or `ApplicationException`.


# Building

Clone a copy of the repo

`git clone https://github.com/CommunityHiQ/Frends.Community.ExecuteProcess.git`

Rebuild the project

`dotnet build`

Run Tests

`dotnet test`

Create a NuGet package

`dotnet pack --configuration Release`

# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

# Change Log

| Version | Changes |
| ------- | ------- |
| 1.6.0   | RunProcess task (Linux compatible) added |
| 1.5.0   | Multiplatform version |
| 1.4.0   | First version |
