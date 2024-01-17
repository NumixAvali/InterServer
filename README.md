# InterServer

This is intermediate server application between solar power generation system, and any smart devices.
Main goal of this project is providing easy, user-friendly, human-readable and machine-readable form.

## API
Secondary goal of this project is to provide API for controlling and monitoring system.

Currently implemented features:
- [ ] Getting information from the hardware
  - [ ] Getting cached information
  - [x] Getting current information
- [ ] Web UI
  - [ ] Graphs
  - [ ] Settings
  - [ ] Auth

## Building project

- Get a [.NET Core 7](https://dotnet.microsoft.com/en-us/) package for your system first;
- Clone this repository to the directory of your choice;
- Open terminal, or PowerShell window in directory with project
and run `dotnet build`
- Put the binaries from `bin/Debug/net7.0/` on any web server of choice
- Start the compiled executable file on the web server

