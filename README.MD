EntityFramework Core to BigQuery 
================
BigQuery.EntityFrameworkCore is C# Entity Framework Core to provide a high-level abstraction over the database for [Google BigQuery](https://cloud.google.com/bigquery/).

Installation
---
To add ntityFramework Core to BigQuery an application, install the NuGet package for the database provider you want to use.

To install or update NuGet packages, you can use the .NET Core command-line interface (CLI), the Visual Studio Package Manager Dialog, or the Visual Studio Package Manager Console.

#### .NET Core CLI
* Use the following .NET Core CLI command from the operating system's command line to install or update the EF Core SQL Server provider:

  ```dotnetcli
  dotnet add package BigQuery.EntityFrameworkCore
  dotnet add package BigQuery.EntityFrameworkCore.DependencyInjection
  ```

* You can indicate a specific version in the `dotnet add package` command, using the `-v` modifier. For example, to install EF Core 2.2.0 packages, append `-v 2.2.0` to the command.

For more information, see [.NET command-line interface (CLI) tools](/dotnet/core/tools/).

### Visual Studio NuGet Package Manager Dialog

* From the Visual Studio menu, select **Project > Manage NuGet Packages**

* Click on the **Browse** or the **Updates** tab

* To install or update the SQL Server provider, select the `Microsoft.EntityFrameworkCore.SqlServer` package, and confirm.

For more information, see [NuGet Package Manager Dialog](/nuget/tools/package-manager-ui).

### Visual Studio NuGet Package Manager Console

* From the Visual Studio menu, select **Tools > NuGet Package Manager > Package Manager Console**

* To install the SQL Server provider, run the following command in the Package Manager Console:

  ```powershell
  Install-Package Microsoft.EntityFrameworkCore.SqlServer
  ```

* To update the provider, use the `Update-Package` command.

* To specify a specific version, use the `-Version` modifier. For example, to install EF Core 2.2.0 packages, append `-Version 2.2.0` to the commands

For more information, see [Package Manager Console](/nuget/tools/package-manager-console).


#### Packages from NuGet:
- [BigQuery.EntityFrameworkCore](https://nuget.org/packages/BigQuery.EntityFrameworkCore)
- [BigQuery.EntityFrameworkCore.DependencyInjection](https://nuget.org/packages/BigQuery.EntityFrameworkCore.DependencyInjection)

#### Package Manager Console
* Use the following .NET Core CLI command from the operating system's command line to install or update the EF Core SQL Server provider:

  ```
  PM> Install-Package BigQuery.EntityFrameworkCore
  PM> Install-Package BigQuery.EntityFrameworkCore.DependencyInjection
  ```

Configuration
---

```

```



Author Info
---
linkedin: https://www.linkedin.com/in/cleber-margarida/

License
---
This library is under MIT License.
