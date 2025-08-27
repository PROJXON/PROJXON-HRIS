# Solution Overview
This is a Human Resources Information System (HRIS) originally built for internal usage by Momentum Intership Program
employees and the Human Resources department. The HRIS consists of a desktop application that allows users to interact
with a server and database hosted on AWS. The HRIS manages employee records and job candidates, and syncs data to the cloud.

## Tech Stack
- **Frontend**: Avalonia with WebView, Community Toolkit MVVM, Refit
- **Backend**: ASP.NET Core, AutoMapper, 
- **Database**: PostgreSQL with Entity Framework Core
- **DevOps**: GitHub Actions
- **Cloud Services**: AWS Elastic Beanstalk, AWS RDS, AWS S3
- **Other Services/Tools**: Docker, Google OAuth, Xunit

# Architecture

# Prerequisites
- Language/runtime: [.NET SDK 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) 
- Tools: [Docker Desktop](https://www.docker.com/products/docker-desktop/), [Git](https://git-scm.com/downloads)

# Setup
## Cloning the Repo
1. Run `git clone git@github.com:PROJXON/PROJXON-HRIS.git`
2. `cd PROJXON-HRIS`
3. Add environment variables and config files from the next section. Be sure to fill in fields that are missing data.
## Environment Variable and Config Files
These files are not saved to source control. Get missing information from existing team members
- A `.env` file in the root of the solution that contains a DB_PASSWORD environment variable to be used by Docker
- A file named `appsettings.Development.json` in the Client project directory with the following content:
  - `{
        "AppName": "Projxon HRIS",
        "Logging": {
            "LogLevel": {
                "Default": "Information"
            }
        },
        "CloudSyncURL": "http://localhost:8080",
        "Auth": {
            "ClientId": "Google OAuth client ID"
        }
    }`
- Another file named `appsettings.Production.json` in the CloudSync project directory with the following content:
  - `{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "JWT": {
    "Key": "Some random string is fine here for development",
    "Issuer": "HRISApp",
    "Audience": "HRISClient",
    "ExpiresInMinutes": 10080
  }
}`

# Running the Project
- **Frontend**: Run `dotnet watch run` in the Client directory
- **Backend**: `docker compose up`

# Testing

# Deployment & CI/CD

# Troubleshooting

# Contributing Guidelines


