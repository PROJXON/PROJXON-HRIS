# Solution Overview


# Additional Requirement
## Necessary Files That Should *not* be Committed to Git
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