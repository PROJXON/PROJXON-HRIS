# Solution Overview
This is a Human Resources Information System (HRIS) originally built for internal usage by Momentum Intership Program
employees and the Human Resources department. The HRIS consists of a desktop application that allows users to interact
with a server and database hosted on AWS. The HRIS manages employee records and job candidates and syncs that data to the cloud.



## Tech Stack
- **Frontend**: Avalonia with WebView, Community Toolkit MVVM, Refit
- **Backend**: ASP.NET Core, AutoMapper, 
- **Database**: PostgreSQL with Entity Framework Core
- **DevOps**: GitHub Actions
- **Cloud Services**: AWS Elastic Beanstalk, AWS RDS, AWS S3
- **Other Services/Tools**: Docker, Google OAuth, Xunit

# Architecture
This HRIS is organized into what .NET calls a solution. A solution is similar to what other languages call a workspace.
A solution allows for multiple .NET projects to be organized and developed alongside each other. The solution keeps
track of which projects fall under its umbrella and helps with their organization.

This solution consists of two main projects, Client and CloudSync. Alongside these, there is a Shared project which
contains classes and contracts that both Client and CloudSync use. Additionally, there are two test projects, one for
Client and one for CloudSync. Below are brief descriptions of the Client, CloudSync, and Shared projects, their structures,
and their role within the overall solution.

## Client
The client project contains the code for the Avalonia desktop frontend application. We're using Avalonia along with
Community Toolkit MVVM. MVVM stands for Model-View-ViewModel, and is a design pattern that separates concerns into 
different layers. 

In MVVM, the View layer  contains XML that defines the appearance of various components, the ViewModel
layer is responsible for event handling from the view, and the ViewModel calls the model layer for data fetching
and API calls to the backend server. More information about this design pattern can be found 
[here](https://docs.avaloniaui.net/docs/concepts/the-mvvm-pattern/). Note that the "Avalonia UI and MVVM" page that comes after the linked page mentions ReactiveUI, but we are
using Community Toolkit instead of ReactiveUI.

## CloudSync


## Shared

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
- `dotnet test`

# Common Tasks
- Adding dependencies: `dotnet add package <package-name>` in the corresponding project directory

# Deployment & CI/CD

# Troubleshooting

# Contributing Guidelines
We utilize feature branching. Feature branching involves creating a branch for each task that a developer works on. The
developer makes changes in that branch, separate from the main branch, and then creates a pull request. A pull request
is essentially a request for code changes to be merged into the main branch. Pull requests are reviewed by another team
member before they can be merged. More information can be found [here](https://www.optimizely.com/optimization-glossary/feature-branch/)
and online. Ask the senior app developer if this is unclear.
- **Branch Naming**: Feature branches should follow the naming convention `<first-name-last-name>/<short-description-of-branch's-purpose>`.
- When finished making changes to a branch:
  1. Make sure that the application still runs in the feature branch. 
  2. Create a pull request to the `main` branch, and request a reviewer (this will most likely be the senior app dev). 
This may change if a `dev` branch is added in the future.
     - PRs need to provide a summary of the changes made in bullet points, including files added or removed.
  3. Reviewer may request that you make changes by commenting within the pull request. Make those changes, then request another review.
  4. Upon successful review, the branch can be merged into the `main` branch.
     - Upon merge, the branch will be automatically deleted.