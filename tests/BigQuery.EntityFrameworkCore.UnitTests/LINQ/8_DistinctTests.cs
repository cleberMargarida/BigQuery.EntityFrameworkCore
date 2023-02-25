using Schemas;

namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ;

public class DistinctTests
{
    private readonly BqTestContext _context;

    public DistinctTests(BqTestContext bqTestContext)
    {
        _context = bqTestContext;
    }

    [Fact]
    public void DataProducts_DistinctToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Distinct().ToString();
        var expected = @"
    SELECT
        DISTINCT Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product";

        Assert.Equal(expected, actual);
    }
}
