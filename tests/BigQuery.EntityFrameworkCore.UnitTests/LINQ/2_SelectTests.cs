using Schemas;

namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ;

public class SelectTests
{
    private readonly BqTestContext _context;

    public SelectTests(BqTestContext context)
    {
        _context = context;
    }

    [Fact]
    public void DataProducts_SelectIdToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Select(x => x.Id).ToString();
        var expected =
    @"
    SELECT
        Product.Id 
    FROM
        data.Product AS Product";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DataProducts_SelectIdAndNameToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Select(x => new { x.Id, x.Name }).ToString();
        var expected = @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DataProducts_SelectIdWhereIdToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Select(x => x.Id).Where(Id => Id == 1).ToString();
        string expected = 
    @"
    SELECT
        Product.Id 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DataProducts_SelectIdOrderByIdToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Select(x => x.Id).OrderBy(Id => Id).ToString();
        string expected =
    @"
    SELECT
        Product.Id 
    FROM
        data.Product AS Product 
    ORDER BY
        Product.Id";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DataProducts_SelectIdOrderByDescendingIdToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Select(x => x.Id).OrderByDescending(Id => Id).ToString();
        var expected =
    @"
    SELECT
        Product.Id 
    FROM
        data.Product AS Product 
    ORDER BY
        Product.Id DESC";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DataProducts_SelectIdTake1ToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Select(x => x.Id).Take(1).ToString();
        var expected =
    @"
    SELECT
        Product.Id 
    FROM
        data.Product AS Product LIMIT 1";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DataProducts_SelectIdTake1Skip1ToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Select(x => x.Id).Take(1).Skip(1).ToString();
        var expected =
    @"
    SELECT
        Product.Id 
    FROM
        data.Product AS Product LIMIT 1 OFFSET 1";

        Assert.Equal(expected, actual);
    }
}
