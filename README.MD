EntityFramework Core to BigQuery 
================
BigQuery.EntityFramework.Core is C# Entity Framework Core Provider for [Google BigQuery](https://cloud.google.com/bigquery/).

Installation
---
package from NuGet, [BigQuery.EntityFramework.Core](https://nuget.org/packages/BigQuery.EntityFramework.Core)

```
PM> Install-Package BigQuery.EntityFramework.Core
```

Configuration
---
Define yours entities using the TableAttribute(**Required**) and KeyAttribute (**OPTIONAL**, will only be used when the entity Id is not specified during the queries).

e.g
```csharp
[Table(projectId: "moonlit-text-367106", dataset: "metadata", tableName: "myentity")]
public class MyEntity
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
}
```
Define a BqContext child class:

``` csharp
public class BigQueryApplicationContext : BqContext
{
    public BigQueryApplicationContext(BigQueryContextBuilder builder) : base(builder) 
    {
    }
    public BqSet<MyEntity> MyEntities { get; set; }
}
```
Operations
---
## Insert
``` csharp
context.MyEntities.Add(myEntity)
```
## Update
``` csharp
//Update. when key attribute is defined.

context.MyEntities.Update(myEntity)
```
``` csharp
//Update all following an expression. key attribute is not necessary.

context.MyEntities.Update(x => x.Id == 1, myEntity);
```
``` csharp
//Update all following a property constant. key attribute is not necessary.

context.MyEntities.Update(x => x.Id, myEntity);
```
## Delete
``` csharp
context.MyEntities.Remove(myEntity);//for this key is required.
```
``` csharp
context.MyEntities.Remove(x => x.Id);
```
``` csharp
context.MyEntities.Remove(x => x.Id == 1);
```
----
#### All these operations will queue up queries to be executed in the big query. For these queries to be dequeued and executed, we need to call the Save method
``` csharp
await context.MyEntities.SaveAsync();
```
Queries
---
All BigQuery functions is under BqFunc.
<br>
 note: BigQuery.EntityFramework.Core is a Linq Provider but is not IQueryable. It's constraint is old good method chain.
#### The rule follows:
```
From((+TableDecorate)+Flatten) -> Join -> Where -| -> OrderBy(ThenBy) -> Select ->                     | -> Limit -> IgnoreCase
                                                 | -> Select | -> GroupBy -> Having -> OrderBy(ThenBy) | -> IgnoreCase
                                                             | -> OrderBy(ThenBy) ->                   |
```
Run/RunAsync - Execute query with QueryResponse.
<br> 
ToList/ToListAsync - Execute query and return rows.
<br> 
AsEnumerable - Execute query and return rows, it's deferred(but resultset is not streaming).
<br> 
ToString - Build query string. It no needs network connection and BigqueryService.
<br> 
Into - Query as Subquery(same as From(query))

## Advanced sample
Code
``` csharp
context.GitHubTimelines
.Where(x => x.repository_language != null && x.repository_fork == "false")
.Select(x => new
{
    x.repository_url,
    x.repository_created_at,
    language = LastValue(x, y => y.repository_language)
        .PartitionBy(y => y.repository_url)
        .OrderBy(y => y.created_at)
        .Value
})
.Into()
.Select(x => new
{
    x.language,
    yyyymm = StrftimeUtcUsec(ParseUtcUsec(x.repository_created_at), "%Y-%m"),
    count = CountDistinct(x.repository_url)
})
.GroupBy(x => new { x.language, x.yyyymm })
.Having(x => GreaterThanEqual(x.yyyymm, "2010-01"))
.Into()
.Select(x => new
{
    x.language,
    x.yyyymm,
    x.count,
    ratio = RatioToReport(x, y => y.count)
        .PartitionBy(y => y.yyyymm)
        .OrderBy(y => y.count)
        .Value
})
.Into()
.Select(x => new
{
    x.language,
    x.count,
    x.yyyymm,
    percentage = Round(x.ratio * 100, 2)
})
.OrderBy(x => x.yyyymm)
.ThenByDescending(x => x.percentage)
.ToList() // ↑BigQuery
.GroupBy(x => x.language)
```
It's query.
``` sql
SELECT
  `language`,
  `count`,
  `yyyymm`,
  ROUND((`ratio` * 100), 2) AS `percentage`
FROM
(
  SELECT
    `language`,
    `yyyymm`,
    `count`,
    RATIO_TO_REPORT(`count`) OVER (PARTITION BY `yyyymm` ORDER BY `count`) AS `ratio`
  FROM
  (
    SELECT
      `language`,
      STRFTIME_UTC_USEC(PARSE_UTC_USEC(`repository_created_at`), '%Y-%m') AS `yyyymm`,
      COUNT(DISTINCT `repository_url`) AS `count`
    FROM
    (
      SELECT
        `repository_url`,
        `repository_created_at`,
        LAST_VALUE(`repository_language`) OVER (PARTITION BY `repository_url` ORDER BY `created_at`) AS `language`
      FROM
        `moonlit-text-367106.data.myentity`
      WHERE
        ((`repository_language` IS NOT NULL) AND (`repository_fork` = 'false'))
    )
    GROUP BY
      `language`,
      `yyyymm`
    HAVING
      `yyyymm` >= '2010-01'
  )
)
ORDER BY
  `yyyymm`, `percentage` DESC
```

### See also, [EFCore-to-BigQuery dependency injection](./BigQuery.EntityFramework.Core.DependencyInjection/README.md)


Author Info
---
linkedin: https://www.linkedin.com/in/cleber-margarida/

License
---
This library is under MIT License.
