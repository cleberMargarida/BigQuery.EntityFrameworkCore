using Schemas;

namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ
{
    public class OrderByDescendingTests
    {
        private readonly BqTestContext _context;

        public OrderByDescendingTests(BqTestContext context)
        {
            _context = context;
        }

        [Fact]
        public void DataProducts_OrderByDescendinIdToString_ShouldReturnExpected()
        {
            var actual = _context.Data.Products.OrderByDescending(x => x.Id).ToString();
            var expected =
@"
    SELECT
        Product.Id,
        Product.ProductName 
    FROM
        data.Product AS Product 
    ORDER BY
        Product.Id DESC";
            Assert.Equal(expected, actual);
        }
    }
}
