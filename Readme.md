# DWPSUtil

A library to support custom powershell cmdlets

At the moment this is only used by my other project: [DWGitsh powershell for Git](https://github.com/DavidWise/DWGitSH)

## Features
- A wrapped base class for PowerShell cmdlets
- A medium complexity caching system
- A process launcher with timeout support
- Support for complex console state management, including colors
- `string[]` objects get an extension method named `TrimAll()` that executes a `.Trim()` 
on each string in the array and returns a copy of the array with all values trimmed

## Supported versions of .Net

- .Net Core 2.2
- .Net Standard 2.0
- .Net Framework 4.8
- .Net Framework 4.7
- .Net Framework 4.6.2
- .Net Framework 4.5

## TODO
- At present, the primary utility class (DWPSUtils) uses only static methods. 
I will be changing that to an instance of a class down the road as this creates the very problem that I seek to solve in my [StaticAbstraction](https://github.com/DavidWise/StaticAbstraction) library