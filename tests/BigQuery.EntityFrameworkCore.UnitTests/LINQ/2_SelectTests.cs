namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ;

public class SelectTests
{
    private readonly BqTestContext _context;
    private readonly Mock<IExecuteQuery> _mock;

    public SelectTests(BqTestContext bqTestContext, Mock<IExecuteQuery> mock)
    {
        _context = bqTestContext;
        _mock = mock;
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


    [Fact]
    public void DataProducts_Count_VerifyExpectedQuery()
    {
        string expected = @"
    SELECT
        COUNT(*) 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1";

        _mock.Setup(x => x.GetResult<int>(expected)).Verifiable();
        _context.Data.Products.Where(x => x.Id == 1).Count();
        _mock.Verify();
        _mock.Reset();
    }

    [Fact]
    public void DataProducts_First_VerifyExpectedQuery()
    {
        string expected = @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1 LIMIT 1";

        _mock.Setup(x => x.GetResult<Product>(expected)).Returns(new Product()).Verifiable();
        _context.Data.Products.Where(x => x.Id == 1).First();
        _mock.Verify();
        _mock.Reset();
    }

    [Fact]
    public void DataProducts_FirstOrDefault_VerifyExpectedQuery()
    {
        string expected = @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1 LIMIT 1";

        _mock.Setup(x => x.GetResult<Product>(expected)).Verifiable();
        _context.Data.Products.Where(x => x.Id == 1).FirstOrDefault();
        _mock.Verify();
        _mock.Reset();
    }

    [Fact]
    public void DataProducts_Single_VerifyExpectedQuery()
    {
        string expected = @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1 LIMIT 1";

        _mock.Setup(x => x.GetResult<Product>(expected)).Returns(new Product()).Verifiable();
        _context.Data.Products.Where(x => x.Id == 1).Single();
        _mock.Verify();
        _mock.Reset();
    }

    [Fact]
    public void DataProducts_SingleOrDefault_VerifyExpectedQuery()
    {
        string expected = @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1 LIMIT 1";

        _mock.Setup(x => x.GetResult<Product>(expected)).Verifiable();
        _context.Data.Products.Where(x => x.Id == 1).SingleOrDefault();
        _mock.Verify();
        _mock.Reset();
    }

    [Fact]
    public void DataProducts_Last_VerifyExpectedQuery()
    {
        string expected = @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1 
        AND Id = (
            
            SELECT
                MAX(Id) 
            FROM
                data.Product AS Product 
            WHERE
                Product.Id = 1
        )";

        _mock.Setup(x => x.GetResult<Product>(expected)).Returns(new Product()).Verifiable();
        _context.Data.Products.Where(x => x.Id == 1).Last();
        _mock.Verify();
        _mock.Reset();
    }

    [Fact]
    public void DataProducts_LastOrDefault_VerifyExpectedQuery()
    {
        string expected = @"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1 
        AND Id = (
            
            SELECT
                MAX(Id) 
            FROM
                data.Product AS Product 
            WHERE
                Product.Id = 1
        )";

        _mock.Setup(x => x.GetResult<Product>(expected)).Verifiable();
        _context.Data.Products.Where(x => x.Id == 1).LastOrDefault();
        _mock.Verify();
        _mock.Reset();
    }

    [Fact]
    public void DataProducts_All_VerifyExpectedQuery()
    {
        string expected = @"
    SELECT
        COUNTIF (Product.Id > 999) = COUNT(*) 
    FROM
        (      
        SELECT
            Product.Id,
            Product.ProductName       
        FROM
            data.Product AS Product       
        WHERE
            Product.Id = 1) AS Product";

        _mock.Setup(x => x.GetResult<bool>(expected)).Verifiable();
        _context.Data.Products.Where(x => x.Id == 1).All(x => x.Id > 999);
        _mock.Verify();
        _mock.Reset();
    }

    [Fact]
    public void DataProducts_Any_VerifyExpectedQuery()
    {
        string expected = @"
    SELECT
        COUNTIF (Product.Id > 999) > 0 
    FROM
        (      
        SELECT
            Product.Id,
            Product.ProductName       
        FROM
            data.Product AS Product       
        WHERE
            Product.Id = 1) AS Product";

        _mock.Setup(x => x.GetResult<bool>(expected)).Verifiable();
        _context.Data.Products.Where(x => x.Id == 1).Any(x => x.Id > 999);
        _mock.Verify();
        _mock.Reset();
    }

    [Fact]
    public void DataProducts_Max_VerifyExpectedQuery()
    {
        string expected = @"
    SELECT
        MAX(Product.Id) 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1";

        _mock.Setup(x => x.GetResult<int>(expected)).Verifiable();
        _context.Data.Products.Where(x => x.Id == 1).Max(x => x.Id);
        _mock.Verify();
        _mock.Reset();
    }

    [Fact]
    public void DataProducts_Min_VerifyExpectedQuery()
    {
        string expected = @"
    SELECT
        MIN(Product.Id) 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1";

        _mock.Setup(x => x.GetResult<int>(expected)).Verifiable();
        _context.Data.Products.Where(x => x.Id == 1).Min(x => x.Id);
        _mock.Verify();
        _mock.Reset();
    }

    [Fact]
    public void DataProducts_Sum_VerifyExpectedQuery()
    {
        string expected = @"
    SELECT
        SUM(Product.Id) 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1";

        _mock.Setup(x => x.GetResult<int>(expected)).Verifiable();
        _context.Data.Products.Where(x => x.Id == 1).Sum(x => x.Id);
        _mock.Verify();
        _mock.Reset();
    }

    [Fact]
    public void DataProducts_Average_VerifyExpectedQuery()
    {
        string expected = @"
    SELECT
        AVG(Product.Id) 
    FROM
        data.Product AS Product 
    WHERE
        Product.Id = 1";

        _mock.Setup(x => x.GetResult<double>(expected)).Verifiable();
        _context.Data.Products.Where(x => x.Id == 1).Average(x => x.Id);
        _mock.Verify();
        _mock.Reset();
    }
}
