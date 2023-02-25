using BigQuery.EntityFrameworkCore.Utils;
using Schemas;

namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ;

public class TableTests
{
    private readonly BqTestContext _context;

    public TableTests(BqTestContext context)
    {
        _context = context;
    }

    [Fact]
    public void DataProducts_ToString_ShouldReturnTheExpectedQuery()
    {
        var actual = _context.Data.Products.ToQueryString();
        var expected =
    @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product";

        Assert.Equal(expected, actual);
    }
}
