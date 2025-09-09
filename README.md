# Solution Overview

This is a Human Resources Information System (HRIS) originally built for internal usage by Momentum Internship Program
employees and the Human Resources department. The HRIS consists of a desktop application that allows users to interact with a server and database hosted on AWS. The HRIS manages employee records and job candidates and syncs that data to the cloud.

## Tech Stack

-   **Frontend**: Avalonia with WebView, Community Toolkit MVVM, Refit
-   **Backend**: ASP.NET Core, AutoMapper
-   **Database**: PostgreSQL with Entity Framework Core (EF Core)
-   **DevOps**: GitHub Actions
-   **Cloud Services**: AWS Elastic Beanstalk, AWS RDS, AWS S3
-   **Other Services/Tools**: Docker, Google OAuth, Xunit

# Architecture

This HRIS is organized into what .NET calls a solution. A solution is similar to what other languages call a workspace. A solution allows for multiple .NET projects to be organized and developed alongside each other. The solution keeps track of which projects fall under its umbrella and helps with their organization.

This solution consists of two main projects, Client and CloudSync. Alongside these, there is a Shared project which
contains classes and contracts that both Client and CloudSync use. Additionally, there are two test projects, one for
Client and one for CloudSync. Below are brief descriptions of the Client, CloudSync, and Shared projects, their
structures, and their role within the overall solution.

## Client

The Client project contains the code for the Avalonia desktop frontend application. We're using Avalonia along with
Community Toolkit MVVM. MVVM stands for Model-View-ViewModel, and is a design pattern that separates concerns into
different layers.

In MVVM, the View layer contains XML that defines the appearance of various components, the ViewModel layer is
responsible for event handling from the view, and the ViewModel calls the model layer for data fetching and API calls to the backend server. More information about this design pattern can be found [here](https://docs.avaloniaui.net/docs/concepts/the-mvvm-pattern/). Note that the "Avalonia UI and MVVM" page that
comes after the linked page mentions ReactiveUI, but we are using Community Toolkit instead of ReactiveUI.

## CloudSync

The CloudSync project contains the code for the web server that allows for syncing of data between the various client
instances. Our backend uses ASP.NET Core, EF Core, AutoMapper, and PostgreSQL, and should be run as a Docker container as described below. The project contains the following files and directories:
-   `Program.cs`: The entry point for the application, which instantiates services, middleware, Swagger, and other necessary resources.
-   `CloudSync.csproj`: The project file. Contains a list of all dependencies and other properties needed to run or publish the project.
-   **Exceptions**: Contains a base exception class and concrete exception types.
-   **Infrastructure**: Currently contains the DatabaseContext (DbContext) config file. A DbContext instance represents a session with the database, and is used within repository classes for CRUD operations.
-   **Middleware**: Contains a class defining how errors/exceptions should be handled.
-   **Migrations**: This directory holds all migrations that have been created by Entity Framework Core using `dotnet ef migration create <migration-name>`. Migrations are like a version control/history for changes to the database models. Each migration holds a record of the changes made at the time of the migration's creation. Migrations are currently automatically applied in `Program.cs`, although this is only safe for development and will need to be changed once the app goes into production.
- **Modules**: This directory holds subdirectories for the parts of the application (modules) that can later be split into microservices, if so desired. Modules are split according to the main features of the app: CandidateManagement, EmployeeManagement, and UserManagement. Each module follows the [controller-service-repository](https://procodeguide.com/programming/repository-pattern-in-aspnet-core/#Repository_Unit_of_Work_Pattern) design pattern, and is split into these subdirectories:
  - Controllers: Defines API endpoints for client interactions.
  - Services: Manipulate data, including mapping to DB models from DTOs and vice-versa thanks to AutoMapper. Contains all other business logic.
  - Repositories: Handle database interactions by calling EF Core methods, fetching or sending data to and from PostgreSQL.
  - Models: Database entity models, which EF Core's DbContext uses to map to database tables and fields.

<figure>
    <img src="https://procodeguide.com/wp-content/uploads/2021/07/Repository-Pattern-in-ASP.NET-Core-Unit-of-Work-1024x432.png"
         alt="Controller-Service-Repository illustration">
    <figcaption>Controller-Service-Repository illustration. Note: The unit of work portion is already handled by EF Core's DbContext feature.</figcaption>
</figure>

## Shared
The Shared project is the simplest project. It only contains enums and data transfer objects (DTOs) that are used by both the CloudSync and Client projects. The DTOs are there to define contracts for frontend and backend interaction, so both know what will be needed in all requests and responses.

These shared resources are separated into directories based on the module they match in the CloudSync project. Base DTOs are defined that contain fields used in both requests and responses, and the request and response classes are for fields that are only used by one or the other.

# Prerequisites

-   Language/runtime: [.NET SDK 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
-   Tools: [Docker Desktop](https://www.docker.com/products/docker-desktop/), [Git](https://git-scm.com/downloads)

# Setup

## Cloning the Repo

1. Run `git clone git@github.com:PROJXON/PROJXON-HRIS.git`
2. `cd PROJXON-HRIS`
3. Run `dotnet restore` in both the CloudSync and Client directories.
4. Add environment variables and config files from the next section. Be sure to fill in fields that are missing data.

## Environment Variable and Config Files

These files are not saved to source control. Get missing information from existing team members

-   A `.env` file in the root of the solution that contains a DB_PASSWORD environment variable to be used by Docker
-   A file named `appsettings.Development.json` in the Client project directory with the following content:
    -   `{ "AppName": "Projxon HRIS", "Logging": { "LogLevel": { "Default": "Information" } }, "CloudSyncURL": "http://localhost:8080", "Auth": { "ClientId": "Google OAuth client ID" } }`
-   Another file named `appsettings.Production.json` in the CloudSync project directory with the following content:
    -   `{ "Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } }, "AllowedHosts": "*", "JWT": { "Key": "Some random string is fine here for development", "Issuer": "HRISApp", "Audience": "HRISClient", "ExpiresInMinutes": 10080 } }`

# Running the Project

-   **Frontend**: Run `dotnet watch run` in the Client directory
-   **Backend**: `docker compose up`

# Testing

-   `dotnet test`

# Common Tasks

-   Adding dependencies: `dotnet add package <package-name>` in the corresponding project directory

# Deployment & CI/CD

Still requires implementation.

# Troubleshooting

# Contributing Guidelines

We utilize feature branching. Feature branching involves creating a branch for each task that a developer works on. The developer makes changes in that branch, separate from the main branch, and then creates a pull request. A pull request is essentially a request for code changes to be merged into the main branch. Pull requests are reviewed by another team member before they can be merged. More information can be found
[here](https://www.optimizely.com/optimization-glossary/feature-branch/) and online. Ask the senior app developer if
this is unclear.

-   **Branch Naming**: Feature branches should follow the naming convention
    `<first-name-last-name>/<short-description-of-branch's-purpose>`.
-   When finished making changes to a branch:
    1. Make sure that the application still runs in the feature branch.
    2. Create a pull request to the `main` branch, and request a reviewer (this will most likely be the senior app dev).
       This may change if a `dev` branch is added in the future. - PRs need to provide a summary of the changes made in bullet points, including files added or removed.
    3. Reviewer may request that you make changes by commenting within the pull request. Make those changes, then
       request another review.
    4. Upon successful review, the branch can be merged into the `main` branch.
        - Upon merge, the branch will be automatically deleted.
