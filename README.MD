EntityFramework Core to BigQuery 
================
BigQuery.EntityFrameworkCore is C# Entity Framework Core to provide a high-level abstraction over the database for [Google BigQuery](https://cloud.google.com/bigquery/).

Installation
---
To add BigQuery EntityFramework Core to an application, install the NuGet package for the bigquery.

#### Packages from NuGet:
- [BigQuery.EntityFrameworkCore](https://nuget.org/packages/BigQuery.EntityFrameworkCore)
- [BigQuery.EntityFrameworkCore.DependencyInjection](https://nuget.org/packages/BigQuery.EntityFrameworkCore.DependencyInjection)

To install or update NuGet packages, you can use the .NET Core command-line interface (CLI), the Visual Studio Package Manager Dialog, or the Visual Studio Package Manager Console.

#### dotnet CLI
* Use the following .NET Core CLI command from the operating system's command line to install or update the EF Core SQL Server provider:

  ```dotnetcli
  dotnet add package BigQuery.EntityFrameworkCore
  dotnet add package BigQuery.EntityFrameworkCore.DependencyInjection
  ```

* You can indicate a specific version in the `dotnet add package` command, using the `-v` modifier.

For more information, see [.NET command-line interface (CLI) tools](/dotnet/core/tools/).

#### Visual Studio NuGet Package Manager Dialog

* From the Visual Studio menu, select **Project > Manage NuGet Packages**

* Click on the **Browse** or the **Updates** tab

* To install or update the BigQuery EntityFrameworkCore provider, select the `BigQuery.EntityFrameworkCore` and package `BigQuery.EntityFrameworkCore.DependencyInjection`, and confirm.

For more information, see [NuGet Package Manager Dialog](/nuget/tools/package-manager-ui).

#### Visual Studio NuGet Package Manager Console

* From the Visual Studio menu, select **Tools > NuGet Package Manager > Package Manager Console**

* To install or update the BigQuery EntityFrameworkCore provider, run the following command in the Package Manager Console:

  ```
  PM> Install-Package BigQuery.EntityFrameworkCore
  PM> Install-Package BigQuery.EntityFrameworkCore.DependencyInjection
  ```

* To update the provider, use the `Update-Package` command.

For more information, see [Package Manager Console](/nuget/tools/package-manager-console).

Configuration
---
* Create your **table** abstractions. The class name as the table name, the properties as the columns. Use if you prefer TableAttrite to specify the table name in the bigquery or ColumnAttribute to specify the column name.
  ```csharp
  [Table("Product")]
  public class Product
  {
      public int Id { get; set; }

      [Column("ProductName")]
      public string Name { get; set; }
  }
  ```
* Create your **MyDataset** class that inherit from Dataset passing as generic parameter your class to abstract the bigquery Dataset containing the table as property. Use if you prefer DatasetAttrite to specify the dataset name in the bigquery.
  ```csharp
  [Dataset("data")]
  public class MyDataset : Dataset<MyDataset>
  {
      public Table<Product> Products { get; set; }
  }
  ```
* Create your **MyBqContext** class that inherit from **BqContext** to abstract the bigquery, and add yours **datasets** abstractions as properties in to the **MyBqContext**:
  ```csharp
  public class MyBqContext : BqContext
  {
      public MyDataset MyDataset { get; set; }
  }
  ```
* In service configurations add the BigQuery EntityFrameworkCore following:
  ``` csharp
  services.AddBqContext<MyBqContext>(x =>
  {
      x.ProjectId = "your-project-id";
      x.GoogleCredential = GoogleCredential.FromJson(Configuration["your_google_auth_key"]);
  });
  ```
Usage
---
With the BqContext properly injected, we can get it in the constructor of the service, store it on a field to use it. 
```csharp
private readonly MyBqContext _context;

public YourService(MyBqContext context)
{
    _context = context;
}
```

  #### Samples
```sql
Enumerator
_context.MyDataset.Products.ToList()
SELECT Id Id, ProductName Name FROM data.Product AS Product

Where
_context.MyDataset.Products.Where(x => x.Id == 1)
SELECT Id Id, ProductName Name FROM data.Product AS Product WHERE Id = 1

Select
_context.MyDataset.Products.Select(x => x.Id)
SELECT Id Id FROM data.Product AS Product

OrderBy
_context.MyDataset.Products.OrderBy(x => x.Id)
SELECT Id Id, ProductName Name FROM data.Product AS Product ORDER BY Id

OrderByDescending
_context.MyDataset.Products.OrderByDescending(x => x.Id)
SELECT Id Id, ProductName Name FROM data.Product AS Product ORDER BY Id DESC
```

And others old good method chain.

Author Info
---
linkedin: https://www.linkedin.com/in/cleber-margarida/

License
---
This library is under MIT License.
