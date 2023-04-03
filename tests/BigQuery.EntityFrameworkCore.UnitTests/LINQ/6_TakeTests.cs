namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ;

public class TakeTests
{
    private readonly BqTestContext _context;

    public TakeTests(BqTestContext bqTestContext)
    {
        _context = bqTestContext;
    }

    [Fact]
    public void DataProducts_Take20ToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Take(20).ToString();
        var expected =
    @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product LIMIT 20";

        Assert.Equal(expected, actual);
    }
}
