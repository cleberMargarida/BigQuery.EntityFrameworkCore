## BigQuery.EntityFrameworkCore extension for Microsoft.Extensions.DependencyInjection

To use, with an `IServiceCollection` instance:

``` csharp
services.AddBqContext<MyBqContext>(x =>
{
	x.ProjectId = "your-project-id";
	x.GoogleCredential = GoogleCredential.FromJson(Configuration["your_google_auth_key"]);
});
```
Usage
---
This registers:

- As a singleton for the `BqContextOptions`
- As a singleton for the `IQueryProvider` service with `BigQueryProvider` impl
- As a singleton for the `BigQueryRowParser`
- As a singleton for the `BigQueryClientFactory`
- `IExpressionPrinter` instances as transient
- `IExecuteQuery` instances as transient
- `Table` instances as scoped
- `Dataset<>` instances as scoped
- `BqContext` instance as scoped

To use at runtime, add a dependency on `BqContext`:

```c#
public class EmployeesController {
	private readonly YourBqContext _context;

	public EmployeesController(YourBqContext context)
		=> _context = context;

	// use the _context
}
```