# AzBuildRunsWebApp

This ASP.NET Core Razor Pages app lists Azure DevOps build runs for a configured organization, project, and user.

## Prerequisites

* .NET SDK 8.0 or later. If the SDK is not available on your system, you can install it locally by running the repository's helper script from the repo root:
  ```bash
  chmod +x ./dotnet-install.sh
  ./dotnet-install.sh --channel 8.0
  export PATH="$PATH:$HOME/.dotnet"
  ```
* Azure CLI logged in with access to the desired Azure DevOps organization.

## Running locally

From the `CSharp/AzBrowser/AzBuildRunsWebApp` directory:

```bash
dotnet build
```

After the build succeeds, start the development server:

```bash
dotnet run --no-build --urls http://0.0.0.0:5000
```

The application will listen on `http://0.0.0.0:5000` by default. Navigate to that URL in your browser to view the build run table. The Azure DevOps organization, project, and user are sourced from `appsettings.json`.

