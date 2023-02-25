using Schemas;

namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ;

public class WhereTests
{
    private readonly BqTestContext _context;

    public WhereTests(BqTestContext context)
    {
        _context = context;
    }

    [Fact]
    public void DataProducts_WhereIdToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Where(x => x.Id == 1).ToString();
        var expected =
    @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DataProducts_WhereIdAndNameToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Where(x => x.Id == 1 && x.Name == "some").ToString();
        var expected =
    @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1 
        AND Product.ProductName = ""some""";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DataProducts_WhereIdSelectIdToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Where(x => x.Id == 1).Select(x => x.Id).ToString();
        var expected =
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
    public void DataProducts_WhereIdOrderByIdToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Where(x => x.Id == 1).OrderBy(x => x.Id).ToString();
        var expected =
    @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1 
    ORDER BY
        Product.Id";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DataProducts_WhereIdOrderByDescendingIdToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Where(x => x.Id == 1).OrderByDescending(x => x.Id).ToString();
        var expected =
    @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1 
    ORDER BY
        Product.Id DESC";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DataProducts_WhereIdTake100ToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Where(x => x.Id == 1).Take(100).ToString();
        var expected =
    @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1 LIMIT 100";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DataProducts_WhereIdTake1Skip1ToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Where(x => x.Id == 1).Take(1).Skip(1).ToString();
        var expected =
    @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1 LIMIT 1 OFFSET 1";

        Assert.Equal(expected, actual);
    }
}
