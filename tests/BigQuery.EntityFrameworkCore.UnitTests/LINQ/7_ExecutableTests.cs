using Moq;
using Schemas;

namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ;

public class ExecutableTests
{
    private readonly BqTestContext _context;
    private readonly Mock<IExecuteQuery> _mock;

    public ExecutableTests(BqTestContext bqTestContext, Mock<IExecuteQuery> mock)
    {
        _context = bqTestContext;
        _mock = mock;
    }

    [Fact]
    public void DataProducts_Count_VerifyExpectedQuery()
    {
        string expected = @"
    SELECT
        COUNT(*) 
    FROM
        data.Product AS Product";

        _mock.Setup(x => x.GetResult<int>(expected)).Verifiable();
        _context.Data.Products.Count();
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
        data.Product AS Product LIMIT 1";

        _mock.Setup(x => x.GetResult<Product>(expected)).Returns(new Product()).Verifiable();
        _context.Data.Products.First();
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
        data.Product AS Product LIMIT 1";

        _mock.Setup(x => x.GetResult<Product>(expected)).Verifiable();
        _context.Data.Products.FirstOrDefault();
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
        data.Product AS Product LIMIT 1";

        _mock.Setup(x => x.GetResult<Product>(expected)).Returns(new Product()).Verifiable();
        _context.Data.Products.Single();
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
        data.Product AS Product LIMIT 1";

        _mock.Setup(x => x.GetResult<Product>(expected)).Verifiable();
        _context.Data.Products.SingleOrDefault();
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
        Id = (
            
            SELECT
                MAX(Id) 
            FROM
                data.Product AS Product
        )";

        _mock.Setup(x => x.GetResult<Product>(expected)).Returns(new Product()).Verifiable();
        _context.Data.Products.Last();
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
        Id = (
            
            SELECT
                MAX(Id) 
            FROM
                data.Product AS Product
        )";

        _mock.Setup(x => x.GetResult<Product>(expected)).Verifiable();
        _context.Data.Products.LastOrDefault();
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
            data.Product AS Product) AS Product";

        _mock.Setup(x => x.GetResult<bool>(expected)).Verifiable();
        _context.Data.Products.All(x => x.Id > 999);
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
            data.Product AS Product) AS Product";

        _mock.Setup(x => x.GetResult<bool>(expected)).Verifiable();
        _context.Data.Products.Any(x => x.Id > 999);
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
        data.Product AS Product";

        _mock.Setup(x => x.GetResult<int>(expected)).Verifiable();
        _context.Data.Products.Max(x => x.Id);
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
        data.Product AS Product";

        _mock.Setup(x => x.GetResult<int>(expected)).Verifiable();
        _context.Data.Products.Min(x => x.Id);
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
        data.Product AS Product";

        _mock.Setup(x => x.GetResult<int>(expected)).Verifiable();
        _context.Data.Products.Sum(x => x.Id);
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
        data.Product AS Product";

        _mock.Setup(x => x.GetResult<double>(expected)).Verifiable();
        _context.Data.Products.Average(x => x.Id);
        _mock.Verify();
        _mock.Reset();
    }
}
