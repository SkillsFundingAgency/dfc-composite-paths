# Digital First Careers – Composite UI - Paths functions

## Introduction

This project provides the Paths functions for use in the Composite UI (Shell application). The Paths are used to identify applications as part of their Application Registration.

Please see https://github.com/SkillsFundingAgency/dfc-composite-paths for additional details about Application Registration.

Details of the Composite UI application may be found here https://github.com/SkillsFundingAgency/dfc-composite-shell

## Getting Started

This is a self-contained Visual Studio 2019 solution containing a number of projects (Azure functions, with associated unit test and integration test projects).

### Installing

Clone the project and open the solution in Visual Studio 2019.

## List of dependencies

|Item	|Purpose|
|-------|-------|
|Azure Cosmos DB | Document storage |

## Local Config Files

Once you have cloned the public repo you need to remove the <i>-template</i> part from the configuration file names listed below.

| Location | Repo Filename | Rename to |
|-------|-------|-------|
| DFC.Composite.Paths.Tests | local.settings-template.json | local.settings.json |
| DFC.Composite.Paths | local.settings-template.json | local.settings.json |

## Configuring to run locally

The project contains a "local.settings-template.json" for the functions, and a "local.settings-template.json" for the tests. These files contain sample appsettings for the functions and the integration test projects. To use these files, rename them to "local.settings.json" and "appsettings.json" as apprppriate and edit and replace the configuration item values with values suitable for your environment.

By default, the appsettings include a local Azure Cosmos Emulator configuration using the well known configuration values. These may be changed to suit your environment if you are not using the Azure Cosmos Emulator. 

## Running locally

To run this product locally, you will need to configure the list of dependencies, once configured and the configuration files updated, it should be F5 to run and debug locally.

To run the project, start the web application. Once running, browse to the Swagger documentation entrypoint which is  "http://localhost:7071/api/Composite Paths/API-Definition". This will describe the functions available and from there, you can use the documentation to make HTTP calls to the Paths functions.

The Paths functions are designed to be run from within the Composite UI.

## Deployments

These Paths functions will be deployed as an individual deployment for consumption by the Composite UI.

## Built With

* Microsoft Visual Studio 2019
* .Net Core 2.2

## References

Please refer to https://github.com/SkillsFundingAgency/dfc-digital for additional instructions on configuring individual components like Sitefinity and Cosmos.
