# Getting Started

This guide will help you get started with ***AdaptiveFlow*** in your project in just a few minutes.

## Prerequisites

- **.NET:** Version 9.0 or higher.
- **Dependencies:** The package uses `Microsoft.Extensions.Logging` and `System.Threading.Channels`. Ensure these dependencies are available in your project. Add them if needed:
    ```bash 
    dotnet add package Microsoft.Extensions.Logging
    ```

## Installation

**1. Add the Package:**
- Install via NuGet:
    ```bash
    dotnet add package AdaptiveFlow
    ```
- Or manually reference the DLL in the .csproj file if using a local version:
    ```xml
    <ItemGroup>
        <Reference Include="AdaptiveFlow">
            <HintPath>path/to/AdaptiveFlow.dll</HintPath>
        </Reference>
    </ItemGroup>
    ```

**2. Basic Configuration:**
- Ensure you have an `IServiceProvider` set up. In ASP.NET Core applications, this is automatically provided via dependency injection.

