namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ;

public class SkipTests
{
    private readonly BqTestContext _context;

    public SkipTests(BqTestContext context)
    {
        _context = context;
    }

    [Fact]
    public void DataProducts_Skip10ToString_ShouldThrowExpectedExceptionAndMessage()
    {
        _context.Data.Products
            .Invoking(x => x.Skip(10).ToString())
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage(ErrorMessages.OffsetClauseWithoutLimitErrorMessage);
    }

    [Fact]
    public void DataProducts_Take20Skip10ToString_ShouldReturnExpected()
    {
        var actual = _context.Data.Products.Take(20).Skip(10).ToString();
        var expected =
    @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product LIMIT 20 OFFSET 10";

        Assert.Equal(expected, actual);
    }
}
